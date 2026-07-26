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
        public static Transform CommonLayer { get; private set; }
        public static Transform CombatOverlay { get; private set; }
        public static Transform PopUILayer { get; private set; }

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
            Transform root = new GameObject("UIRoot").transform;
            root.SetParent(transform);

            GameObject bgObj = new("BgLayer", typeof(RectTransform));
            bgObj.transform.SetParent(root);
            Canvas bgCanvas = bgObj.AddComponent<Canvas>();
            bgCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            bgCanvas.sortingOrder = 0;
            SetupCanvas(bgObj);

            GameObject sceneContainerObj = new("SceneContainer", typeof(RectTransform));
            sceneContainerObj.transform.SetParent(root);
            Canvas scCanvas = sceneContainerObj.AddComponent<Canvas>();
            scCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            scCanvas.sortingOrder = 5;
            SetupCanvas(sceneContainerObj);
            SceneContainer sceneContainer = sceneContainerObj.AddComponent<SceneContainer>();

            CommonLayer = CreateLayer("CommonLayer", root, 10);
            CombatOverlay = CreateLayer("CombatOverlay", root, 15);
            PopUILayer = CreateLayer("PopUILayer", root, 20);

            GameMain.Interface.RegisterSystem<ISceneManager>(new SceneManager(sceneContainer));
        }

        private static Transform CreateLayer(string name, Transform parent, int sortOrder)
        {
            GameObject obj = new(name, typeof(RectTransform));
            obj.transform.SetParent(parent);
            Canvas canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            SetupCanvas(obj);
            return obj.transform;
        }

        private static void SetupCanvas(GameObject obj)
        {
            CanvasScaler scaler = obj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f;

            obj.AddComponent<GraphicRaycaster>();
            Stretch((RectTransform)obj.transform);
        }

        private static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
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