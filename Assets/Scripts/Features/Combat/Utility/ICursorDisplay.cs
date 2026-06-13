using QFramework;
using UnityEngine;

namespace Features.Combat.Utility
{
    public interface ICursorDisplay : IUtility
    {
        void ShowAt(Vector3 position);
        void Hide();
    }
}