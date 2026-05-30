// asmdef Version Defines, enabled when com.unity.addressables is imported.

#if UNITASK_ADDRESSABLE_SUPPORT

using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using Cysharp.Threading.Tasks.Internal;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Cysharp.Threading.Tasks
{
    public static class AddressablesAsyncExtensions
    {
        #region AsyncOperationHandle
        public static UniTask.Awaiter GetAwaiter(this AsyncOperationHandle handle)
        {
            return handle.ToUniTask().GetAwaiter();
        }

        public static UniTask WithCancellation(this AsyncOperationHandle handle, CancellationToken cancellationToken, bool cancelImmediately = false, bool autoReleaseWhenCanceled = false)
        {
            return handle.ToUniTask(cancellationToken: cancellationToken, cancelImmediately: cancelImmediately, autoReleaseWhenCanceled: autoReleaseWhenCanceled);
        }

        public static UniTask ToUniTask(this AsyncOperationHandle handle, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update,
            CancellationToken cancellationToken = default, bool cancelImmediately = false, bool autoReleaseWhenCanceled = false)
        {
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled(cancellationToken);

            if (!handle.IsValid())
            {
                // autoReleaseHandle:true handle is invalid(immediately internal handle == null) so return completed.
                return UniTask.CompletedTask;
            }

            if (handle.IsDone)
            {
                if (handle.Status == AsyncOperationStatus.Failed)
                {
                    return UniTask.FromException(handle.OperationException);
                }

                return UniTask.CompletedTask;
            }

            return new UniTask(AsyncOperationHandleConfiguredSource.Create(handle, timing, progress, cancellationToken, cancelImmediately, autoReleaseWhenCanceled, out short token), token);
        }

        public struct AsyncOperationHandleAwaiter : ICriticalNotifyCompletion
        {
            private AsyncOperationHandle handle;
            private Action<AsyncOperationHandle> continuationAction;

            public AsyncOperationHandleAwaiter(AsyncOperationHandle handle)
            {
                this.handle = handle;
                continuationAction = null;
            }

            public bool IsCompleted { get => handle.IsDone; }

            public void GetResult()
            {
                if (continuationAction != null)
                {
                    handle.Completed -= continuationAction;
                    continuationAction = null;
                }

                if (handle.Status == AsyncOperationStatus.Failed)
                {
                    Exception e = handle.OperationException;
                    handle = default;
                    ExceptionDispatchInfo.Capture(e).Throw();
                }

                object result = handle.Result;
                handle = default;
            }

            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                Error.ThrowWhenContinuationIsAlreadyRegistered(continuationAction);
                continuationAction = PooledDelegate<AsyncOperationHandle>.Create(continuation);
                handle.Completed += continuationAction;
            }
        }

        private sealed class AsyncOperationHandleConfiguredSource : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<AsyncOperationHandleConfiguredSource>
        {
            private static TaskPool<AsyncOperationHandleConfiguredSource> pool;
            private AsyncOperationHandleConfiguredSource nextNode;

            public ref AsyncOperationHandleConfiguredSource NextNode { get => ref nextNode; }

            static AsyncOperationHandleConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(AsyncOperationHandleConfiguredSource), () => pool.Size);
            }

            private readonly Action<AsyncOperationHandle> completedCallback;
            private AsyncOperationHandle handle;
            private CancellationToken cancellationToken;
            private CancellationTokenRegistration cancellationTokenRegistration;
            private IProgress<float> progress;
            private bool autoReleaseWhenCanceled;
            private bool cancelImmediately;
            private bool completed;

            private UniTaskCompletionSourceCore<AsyncUnit> core;

            private AsyncOperationHandleConfiguredSource()
            {
                completedCallback = HandleCompleted;
            }

            public static IUniTaskSource Create(AsyncOperationHandle handle, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately,
                bool autoReleaseWhenCanceled, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out AsyncOperationHandleConfiguredSource result))
                {
                    result = new AsyncOperationHandleConfiguredSource();
                }

                result.handle = handle;
                result.progress = progress;
                result.cancellationToken = cancellationToken;
                result.cancelImmediately = cancelImmediately;
                result.autoReleaseWhenCanceled = autoReleaseWhenCanceled;
                result.completed = false;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        AsyncOperationHandleConfiguredSource promise = (AsyncOperationHandleConfiguredSource)state;
                        if (promise.autoReleaseWhenCanceled && promise.handle.IsValid())
                        {
                            Addressables.Release(promise.handle);
                        }

                        promise.core.TrySetCanceled(promise.cancellationToken);
                    }, result);
                }

                TaskTracker.TrackActiveTask(result, 3);

                PlayerLoopHelper.AddAction(timing, result);

                handle.Completed += result.completedCallback;

                token = result.core.Version;
                return result;
            }

            private void HandleCompleted(AsyncOperationHandle _)
            {
                if (handle.IsValid())
                {
                    handle.Completed -= completedCallback;
                }

                if (completed)
                {
                    return;
                }

                completed = true;
                if (cancellationToken.IsCancellationRequested)
                {
                    if (autoReleaseWhenCanceled && handle.IsValid())
                    {
                        Addressables.Release(handle);
                    }

                    core.TrySetCanceled(cancellationToken);
                }
                else if (handle.Status == AsyncOperationStatus.Failed)
                {
                    core.TrySetException(handle.OperationException);
                }
                else
                {
                    core.TrySetResult(AsyncUnit.Default);
                }
            }

            public void GetResult(short token)
            {
                try
                {
                    core.GetResult(token);
                }
                finally
                {
                    if (!(cancelImmediately && cancellationToken.IsCancellationRequested))
                    {
                        TryReturn();
                    }
                    else
                    {
                        TaskTracker.RemoveTracking(this);
                    }
                }
            }

            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public bool MoveNext()
            {
                if (completed)
                {
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    completed = true;
                    if (autoReleaseWhenCanceled && handle.IsValid())
                    {
                        Addressables.Release(handle);
                    }

                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (progress != null && handle.IsValid())
                {
                    progress.Report(handle.GetDownloadStatus().Percent);
                }

                return true;
            }

            private bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                handle = default;
                progress = default;
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                return pool.TryPush(this);
            }
        }
        #endregion

        #region AsyncOperationHandle_T
        public static UniTask<T>.Awaiter GetAwaiter<T>(this AsyncOperationHandle<T> handle)
        {
            return handle.ToUniTask().GetAwaiter();
        }

        public static UniTask<T> WithCancellation<T>(this AsyncOperationHandle<T> handle, CancellationToken cancellationToken, bool cancelImmediately = false,
            bool autoReleaseWhenCanceled = false)
        {
            return handle.ToUniTask(cancellationToken: cancellationToken, cancelImmediately: cancelImmediately, autoReleaseWhenCanceled: autoReleaseWhenCanceled);
        }

        public static UniTask<T> ToUniTask<T>(this AsyncOperationHandle<T> handle, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update,
            CancellationToken cancellationToken = default, bool cancelImmediately = false, bool autoReleaseWhenCanceled = false)
        {
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled<T>(cancellationToken);

            if (!handle.IsValid())
            {
                throw new Exception("Attempting to use an invalid operation handle");
            }

            if (handle.IsDone)
            {
                if (handle.Status == AsyncOperationStatus.Failed)
                {
                    return UniTask.FromException<T>(handle.OperationException);
                }

                return UniTask.FromResult(handle.Result);
            }

            return new UniTask<T>(AsyncOperationHandleConfiguredSource<T>.Create(handle, timing, progress, cancellationToken, cancelImmediately, autoReleaseWhenCanceled, out short token),
                token);
        }

        private sealed class AsyncOperationHandleConfiguredSource<T> : IUniTaskSource<T>, IPlayerLoopItem, ITaskPoolNode<AsyncOperationHandleConfiguredSource<T>>
        {
            private static TaskPool<AsyncOperationHandleConfiguredSource<T>> pool;
            private AsyncOperationHandleConfiguredSource<T> nextNode;

            public ref AsyncOperationHandleConfiguredSource<T> NextNode { get => ref nextNode; }

            static AsyncOperationHandleConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(AsyncOperationHandleConfiguredSource<T>), () => pool.Size);
            }

            private readonly Action<AsyncOperationHandle<T>> completedCallback;
            private AsyncOperationHandle<T> handle;
            private CancellationToken cancellationToken;
            private CancellationTokenRegistration cancellationTokenRegistration;
            private IProgress<float> progress;
            private bool autoReleaseWhenCanceled;
            private bool cancelImmediately;
            private bool completed;

            private UniTaskCompletionSourceCore<T> core;

            private AsyncOperationHandleConfiguredSource()
            {
                completedCallback = HandleCompleted;
            }

            public static IUniTaskSource<T> Create(AsyncOperationHandle<T> handle, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken,
                bool cancelImmediately, bool autoReleaseWhenCanceled, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<T>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out AsyncOperationHandleConfiguredSource<T> result))
                {
                    result = new AsyncOperationHandleConfiguredSource<T>();
                }

                result.handle = handle;
                result.cancellationToken = cancellationToken;
                result.completed = false;
                result.progress = progress;
                result.autoReleaseWhenCanceled = autoReleaseWhenCanceled;
                result.cancelImmediately = cancelImmediately;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        AsyncOperationHandleConfiguredSource<T> promise = (AsyncOperationHandleConfiguredSource<T>)state;
                        if (promise.autoReleaseWhenCanceled && promise.handle.IsValid())
                        {
                            Addressables.Release(promise.handle);
                        }

                        promise.core.TrySetCanceled(promise.cancellationToken);
                    }, result);
                }

                TaskTracker.TrackActiveTask(result, 3);

                PlayerLoopHelper.AddAction(timing, result);

                handle.Completed += result.completedCallback;

                token = result.core.Version;
                return result;
            }

            private void HandleCompleted(AsyncOperationHandle<T> argHandle)
            {
                if (handle.IsValid())
                {
                    handle.Completed -= completedCallback;
                }

                if (completed)
                {
                    return;
                }

                completed = true;
                if (cancellationToken.IsCancellationRequested)
                {
                    if (autoReleaseWhenCanceled && handle.IsValid())
                    {
                        Addressables.Release(handle);
                    }

                    core.TrySetCanceled(cancellationToken);
                }
                else if (argHandle.Status == AsyncOperationStatus.Failed)
                {
                    core.TrySetException(argHandle.OperationException);
                }
                else
                {
                    core.TrySetResult(argHandle.Result);
                }
            }

            public T GetResult(short token)
            {
                try
                {
                    return core.GetResult(token);
                }
                finally
                {
                    if (!(cancelImmediately && cancellationToken.IsCancellationRequested))
                    {
                        TryReturn();
                    }
                    else
                    {
                        TaskTracker.RemoveTracking(this);
                    }
                }
            }

            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public bool MoveNext()
            {
                if (completed)
                {
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    completed = true;
                    if (autoReleaseWhenCanceled && handle.IsValid())
                    {
                        Addressables.Release(handle);
                    }

                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (progress != null && handle.IsValid())
                {
                    progress.Report(handle.GetDownloadStatus().Percent);
                }

                return true;
            }

            private bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                handle = default;
                progress = default;
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                return pool.TryPush(this);
            }
        }
        #endregion
    }
}

#endif