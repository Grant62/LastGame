using Core.Architecture;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;

namespace Core.SceneManagement
{
    public class SceneContainer : MonoBehaviour, IController
    {
        public SceneBase CurrentScene { get; private set; }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public async UniTask SetCurrentScene(SceneBase newScene, SceneLoadContext ctx = null,
            bool withTransition = true)
        {
            ISceneTransition transition = null;

            try
            {
                transition = this.GetUtility<ISceneTransition>();
            }
            catch { }

            if (withTransition && transition != null)
                await transition.FadeOut();

            if (CurrentScene != null)
            {
                await CurrentScene.OnSceneExit();
                DestroyImmediate(CurrentScene.gameObject);
                CurrentScene = null;
            }

            if (newScene != null)
            {
                newScene.transform.SetParent(transform, false);
                await newScene.OnSceneEnter(ctx ?? SceneLoadContext.Empty);
                CurrentScene = newScene;
            }

            if (withTransition && transition != null)
                await transition.FadeIn();
        }

        public void Clear()
        {
            if (CurrentScene != null)
            {
                DestroyImmediate(CurrentScene.gameObject);
                CurrentScene = null;
            }
        }
    }
}