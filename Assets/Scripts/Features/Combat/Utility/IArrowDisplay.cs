using QFramework;
using UnityEngine;

namespace Features.Combat.Utility
{
    public interface IArrowDisplay : IUtility
    {
        void Show(Vector3 startPosition);
        void Hide();
        void UpdateMouse(Vector3 mousePos);
    }
}