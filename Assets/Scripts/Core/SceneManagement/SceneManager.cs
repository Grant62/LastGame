using Core.SceneManagement.Event;
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
        private SceneContainer mRoomContainer;

        public SceneBase CurrentMainScene { get => mMainContainer.CurrentScene; }

        public SceneBase CurrentRoomScene { get => mRoomContainer != null ? mRoomContainer.CurrentScene : null; }

        public SceneManager(SceneContainer mainContainer)
        {
            mMainContainer = mainContainer;
        }

        public void SetRoomContainer(SceneContainer roomContainer)
        {
            mRoomContainer = roomContainer;
        }

        protected override void OnInit() { }

        public async UniTask LoadMainScene(string sceneId, SceneLoadContext ctx = null)
        {
            if (mMainContainer.CurrentScene != null)
                this.SendEvent(new SceneExitedEvent { SceneId = mMainContainer.CurrentScene.SceneId });

            await mMainContainer.SetCurrentScene(null);

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(sceneId);
            GameObject prefab = await handle.Task;
            GameObject instance = Object.Instantiate(prefab);
            SceneBase scene = instance.GetComponent<SceneBase>();

            await mMainContainer.SetCurrentScene(scene, ctx);

            this.SendEvent(new SceneReadyEvent { SceneId = sceneId });

            Addressables.Release(handle);
        }

        public async UniTask LoadRoomScene(string sceneId, SceneLoadContext ctx = null)
        {
            if (mRoomContainer == null)
            {
                Debug.LogError("[SceneManager] LoadRoomScene called but mRoomContainer is null. Call SetRoomContainer first.");
                return;
            }

            if (mRoomContainer.CurrentScene != null)
                this.SendEvent(new RoomExitedEvent { RoomId = mRoomContainer.CurrentScene.SceneId });

            await mRoomContainer.SetCurrentScene(null);

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(sceneId);
            GameObject prefab = await handle.Task;
            GameObject instance = Object.Instantiate(prefab);
            SceneBase scene = instance.GetComponent<SceneBase>();

            await mRoomContainer.SetCurrentScene(scene, ctx);

            this.SendEvent(new RoomReadyEvent { RoomId = sceneId });

            Addressables.Release(handle);
        }

        public async UniTask PreloadScene(string sceneId)
        {
            await UniTask.CompletedTask;
        }
    }
}