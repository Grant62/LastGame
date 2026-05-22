using QFramework;
using UnityEngine;

namespace Features.Combat.Targeting
{
    public interface IArrowDisplay : IUtility
    {
        void Show(Vector3 startPosition);
        void Hide();
    }
}