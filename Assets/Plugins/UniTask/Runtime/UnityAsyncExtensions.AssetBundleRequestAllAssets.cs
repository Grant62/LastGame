#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

#if UNITY_2018_4 || UNITY_2019_4_OR_NEWER
#if UNITASK_ASSETBUNDLE_SUPPORT

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks.Internal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cysharp.Threading.Tasks
{
    public static partial class UnityAsyncExtensions
    {
        public static AssetBundleRequestAllAssetsAwaiter AwaitForAllAssets(this AssetBundleRequest asyncOperation)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            return new AssetBundleRequestAllAssetsAwaiter(asyncOperation);
        }

        public static UniTask<Object[]> AwaitForAllAssets(this AssetBundleRequest asyncOperation, CancellationToken cancellationToken)
        {
            return asyncOperation.AwaitForAllAssets(null, PlayerLoopTiming.Update, cancellationToken);
        }

        public static UniTask<Object[]> AwaitForAllAssets(this AssetBundleRequest asyncOperation, CancellationToken cancellationToken, bool cancelImmediately)
        {
            return asyncOperation.AwaitForAllAssets(null, cancellationToken: cancellationToken, cancelImmediately: cancelImmediately);
        }

        public static UniTask<Object[]> AwaitForAllAssets(this AssetBundleRequest asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update,
            CancellationToken cancellationToken = default, bool cancelImmediately = false)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled<Object[]>(cancellationToken);
            if (asyncOperation.isDone) return UniTask.FromResult(asyncOperation.allAssets);
            return new UniTask<Object[]>(AssetBundleRequestAllAssetsConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, cancelImmediately, out short token), token);
        }

        public struct AssetBundleRequestAllAssetsAwaiter : ICriticalNotifyCompletion
        {
            private AssetBundleRequest asyncOperation;
            private Action<AsyncOperation> continuationAction;

            public AssetBundleRequestAllAssetsAwaiter(AssetBundleRequest asyncOperation)
            {
                this.asyncOperation = asyncOperation;
                continuationAction = null;
            }

            public AssetBundleRequestAllAssetsAwaiter GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted { get => asyncOperation.isDone; }

            public Object[] GetResult()
            {
                if (continuationAction != null)
                {
                    asyncOperation.completed -= continuationAction;
                    continuationAction = null;
                    Object[] result = asyncOperation.allAssets;
                    asyncOperation = null;
                    return result;
                }
                else
                {
                    Object[] result = asyncOperation.allAssets;
                    asyncOperation = null;
                    return result;
                }
            }

            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                Error.ThrowWhenContinuationIsAlreadyRegistered(continuationAction);
                continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
                asyncOperation.completed += continuationAction;
            }
        }

        private sealed class AssetBundleRequestAllAssetsConfiguredSource : IUniTaskSource<Object[]>, IPlayerLoopItem, ITaskPoolNode<AssetBundleRequestAllAssetsConfiguredSource>
        {
            private static TaskPool<AssetBundleRequestAllAssetsConfiguredSource> pool;
            private AssetBundleRequestAllAssetsConfiguredSource nextNode;

            public ref AssetBundleRequestAllAssetsConfiguredSource NextNode { get => ref nextNode; }

            static AssetBundleRequestAllAssetsConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(AssetBundleRequestAllAssetsConfiguredSource), () => pool.Size);
            }

            private AssetBundleRequest asyncOperation;
            private IProgress<float> progress;
            private CancellationToken cancellationToken;
            private CancellationTokenRegistration cancellationTokenRegistration;
            private bool cancelImmediately;
            private bool completed;

            private UniTaskCompletionSourceCore<Object[]> core;

            private readonly Action<AsyncOperation> continuationAction;

            private AssetBundleRequestAllAssetsConfiguredSource()
            {
                continuationAction = Continuation;
            }

            public static IUniTaskSource<Object[]> Create(AssetBundleRequest asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken,
                bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<Object[]>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out AssetBundleRequestAllAssetsConfiguredSource result))
                {
                    result = new AssetBundleRequestAllAssetsConfiguredSource();
                }

                result.asyncOperation = asyncOperation;
                result.progress = progress;
                result.cancellationToken = cancellationToken;
                result.cancelImmediately = cancelImmediately;
                result.completed = false;

                asyncOperation.completed += result.continuationAction;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        AssetBundleRequestAllAssetsConfiguredSource source = (AssetBundleRequestAllAssetsConfiguredSource)state;
                        source.core.TrySetCanceled(source.cancellationToken);
                    }, result);
                }

                TaskTracker.TrackActiveTask(result, 3);

                PlayerLoopHelper.AddAction(timing, result);

                token = result.core.Version;
                return result;
            }

            public Object[] GetResult(short token)
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
                // Already completed 
                if (completed || asyncOperation == null)
                {
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (progress != null)
                {
                    progress.Report(asyncOperation.progress);
                }

                if (asyncOperation.isDone)
                {
                    core.TrySetResult(asyncOperation.allAssets);
                    return false;
                }

                return true;
            }

            private bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                asyncOperation = default;
                progress = default;
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                cancelImmediately = default;
                return pool.TryPush(this);
            }

            private void Continuation(AsyncOperation _)
            {
                if (completed)
                {
                    return;
                }

                completed = true;
                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                }
                else
                {
                    core.TrySetResult(asyncOperation.allAssets);
                }
            }
        }
    }
}

#endif
#endif