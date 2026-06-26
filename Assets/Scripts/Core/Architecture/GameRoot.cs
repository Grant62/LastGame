using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Architecture
{
    [MonoSingletonPath("GameRoot")]
    public class GameRoot : PersistentMonoSingleton<GameRoot>
    {
        protected override void Awake()
        {
            base.Awake();
            IArchitecture _ = GameMain.Interface;
            BuildRootCanvas();
        }

        private void Start()
        {
            DoStart().Forget();
        }

        private async UniTaskVoid DoStart()
        {
            await GameMain.Interface.GetSystem<ISceneManager>().LoadMainScene("LogoSceneRoot");
        }

        private void BuildRootCanvas()
        {
            Canvas rootCanvas = new GameObject("RootCanvas").AddComponent<Canvas>();
            rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            rootCanvas.transform.SetParent(transform);
            rootCanvas.gameObject.AddComponent<CanvasScaler>();
            rootCanvas.gameObject.AddComponent<GraphicRaycaster>();

            Transform bgLayer = new GameObject("BgLayer").transform;
            bgLayer.SetParent(rootCanvas.transform);

            GameObject sceneContainerObj = new("SceneContainer");
            sceneContainerObj.transform.SetParent(rootCanvas.transform);
            SceneContainer sceneContainer = sceneContainerObj.AddComponent<SceneContainer>();

            GameObject overlayContainerObj = new("OverlayContainer");
            overlayContainerObj.transform.SetParent(rootCanvas.transform);
            SceneContainer overlayContainer = overlayContainerObj.AddComponent<SceneContainer>();

            GameObject transitionObj = new("TransitionLayer");
            Image transitionImage = transitionObj.AddComponent<Image>();
            transitionImage.color = Color.black;
            transitionImage.raycastTarget = true;
            transitionImage.gameObject.SetActive(false);
            RectTransform transitionRect = transitionObj.transform as RectTransform;
            transitionRect.SetParent(rootCanvas.transform, false);
            transitionRect.anchorMin = Vector2.zero;
            transitionRect.anchorMax = Vector2.one;
            transitionRect.sizeDelta = Vector2.zero;

            GameMain.Interface.RegisterUtility<ISceneTransition>(new SceneTransition(transitionImage));
            GameMain.Interface.RegisterSystem<ISceneManager>(new SceneManager(sceneContainer, overlayContainer));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInit()
        {
            if (FindAnyObjectByType<GameRoot>() == null)
            {
                new GameObject("GameRoot").AddComponent<GameRoot>();
            }
        }
    }
}