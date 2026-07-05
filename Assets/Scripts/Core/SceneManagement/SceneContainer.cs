using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.SceneManagement
{
    public class SceneContainer : MonoBehaviour
    {
        public SceneBase CurrentScene { get; private set; }

        public async UniTask SetCurrentScene(SceneBase newScene, SceneLoadContext ctx = null)
        {
            if (CurrentScene != null)
            {
                await CurrentScene.OnSceneExit();
                Destroy(CurrentScene.gameObject);
                CurrentScene = null;
            }

            if (newScene != null)
            {
                newScene.transform.SetParent(transform, false);
                await newScene.OnSceneEnter(ctx ?? SceneLoadContext.Empty);
                CurrentScene = newScene;
            }
        }

        public void Clear()
        {
            if (CurrentScene != null)
            {
                Destroy(CurrentScene.gameObject);
                CurrentScene = null;
            }
        }
    }
}