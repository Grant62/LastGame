using QFramework;
using UnityEngine;

namespace Features.Combat.UI
{
    public interface IPotionTooltip : IUtility
    {
        void Show(cfg.PotionInfo potion, Vector3 position);
        void Hide();
    }
}
