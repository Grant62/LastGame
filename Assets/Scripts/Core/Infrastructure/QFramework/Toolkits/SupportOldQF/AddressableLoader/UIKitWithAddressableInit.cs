using QFramework;
using UnityEngine;

namespace Core.Infrastructure.QFramework.Toolkits.SupportOldQF
{
    public class UIKitWithAddressableInit
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            UIKit.Config.PanelLoaderPool = new AddressablePanelLoaderPool();
        }
    }
}
