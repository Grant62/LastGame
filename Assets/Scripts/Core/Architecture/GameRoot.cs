using QFramework;
using UnityEngine;

namespace Core.Architecture
{
    [MonoSingletonPath("GameRoot")]
    public class GameRoot : PersistentMonoSingleton<GameRoot>
    {
        protected override void Awake()
        {
            base.Awake();
            IArchitecture _ = GameMain.Interface;
            LogKit.I("GameMain initialized");
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