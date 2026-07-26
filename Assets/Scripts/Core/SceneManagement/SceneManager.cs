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

        public UniTask LoadMainScene(string sceneId, SceneLoadContext ctx = null)
        {
            return LoadScene(sceneId, mMainContainer, ctx, isMain: true);
        }

        public UniTask LoadRoomScene(string sceneId, SceneLoadContext ctx = null)
        {
            if (mRoomContainer == null)
                return UniTask.CompletedTask;

            return LoadScene(sceneId, mRoomContainer, ctx, isMain: false);
        }

        private async UniTask LoadScene(string sceneId, SceneContainer container, SceneLoadContext ctx, bool isMain)
        {
            if (container.CurrentScene != null)
            {
                string exitingId = container.CurrentScene.SceneId;
                if (isMain)
                    this.SendEvent(new SceneExitedEvent { SceneId = exitingId });
                else
                    this.SendEvent(new RoomExitedEvent { RoomId = exitingId });
            }

            await container.SetCurrentScene(null);

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(sceneId);
            GameObject prefab = await handle.Task;
            GameObject instance = Object.Instantiate(prefab);
            SceneBase scene = instance.GetComponent<SceneBase>();

            if (scene == null)
            {
                Object.Destroy(instance);
                Addressables.Release(handle);
                return;
            }

            await container.SetCurrentScene(scene, ctx);

            if (isMain)
                this.SendEvent(new SceneReadyEvent { SceneId = sceneId });
            else
                this.SendEvent(new RoomReadyEvent { RoomId = sceneId });

            Addressables.Release(handle);
        }
    }
}
