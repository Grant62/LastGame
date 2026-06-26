using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.SceneManagement
{
    public class SceneManager : AbstractSystem, ISceneManager
    {
        private readonly SceneContainer mMainContainer;
        private readonly SceneContainer mOverlayContainer;
        private SceneContainer mRoomContainer;

        public SceneBase CurrentMainScene { get => mMainContainer.CurrentScene; }

        public SceneBase CurrentRoomScene { get => mRoomContainer != null ? mRoomContainer.CurrentScene : null; }

        public SceneManager(SceneContainer mainContainer, SceneContainer overlayContainer)
        {
            mMainContainer = mainContainer;
            mOverlayContainer = overlayContainer;
        }

        public void SetRoomContainer(SceneContainer roomContainer)
        {
            mRoomContainer = roomContainer;
        }

        protected override void OnInit() { }

        public async UniTask LoadMainScene(string sceneId, SceneLoadContext ctx = null,
            bool withTransition = false)
        {
            if (mMainContainer.CurrentScene != null)
                this.SendEvent(new SceneExitedEvent { SceneId = mMainContainer.CurrentScene.SceneId });

            await mMainContainer.SetCurrentScene(null, null, false);

            if (withTransition)
                this.GetUtility<ISceneTransition>().SetImmediate(true);

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(sceneId);
            GameObject prefab = await handle.Task;
            GameObject instance = Object.Instantiate(prefab);
            SceneBase scene = instance.GetComponent<SceneBase>();

            await mMainContainer.SetCurrentScene(scene, ctx, withTransition);

            this.SendEvent(new SceneReadyEvent { SceneId = sceneId });

            Addressables.Release(handle);
        }

        public async UniTask LoadRoomScene(string sceneId, SceneLoadContext ctx = null)
        {
            if (mRoomContainer == null)
                return;

            if (mRoomContainer.CurrentScene != null)
                this.SendEvent(new RoomExitedEvent { RoomId = mRoomContainer.CurrentScene.SceneId });

            await mRoomContainer.SetCurrentScene(null, null, false);

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(sceneId);
            GameObject prefab = await handle.Task;
            GameObject instance = Object.Instantiate(prefab);
            SceneBase scene = instance.GetComponent<SceneBase>();

            await mRoomContainer.SetCurrentScene(scene, ctx);

            this.SendEvent(new RoomReadyEvent { RoomId = sceneId });

            Addressables.Release(handle);
        }

        public async UniTask ShowOverlay(string overlayId, SceneLoadContext ctx = null)
        {
            if (mMainContainer.CurrentScene != null)
                mMainContainer.CurrentScene.OnScenePause();

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(overlayId);
            GameObject prefab = await handle.Task;
            GameObject instance = Object.Instantiate(prefab);
            SceneBase scene = instance.GetComponent<SceneBase>();

            scene.transform.SetParent(mOverlayContainer.transform, false);
            await scene.OnSceneEnter(ctx ?? SceneLoadContext.Empty);

            Addressables.Release(handle);
        }

        public async UniTask HideOverlay()
        {
            if (mOverlayContainer.CurrentScene != null)
            {
                await mOverlayContainer.CurrentScene.OnSceneExit();
                Object.Destroy(mOverlayContainer.CurrentScene.gameObject);
            }

            if (mMainContainer.CurrentScene != null)
                mMainContainer.CurrentScene.OnSceneResume();
        }

        public async UniTask PreloadScene(string sceneId)
        {
            await UniTask.CompletedTask;
        }
    }
}