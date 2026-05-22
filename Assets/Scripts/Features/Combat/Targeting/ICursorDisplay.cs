using QFramework;
using UnityEngine;

namespace Features.Combat.Targeting
{
    public interface ICursorDisplay : IUtility
    {
        void ShowAt(Vector3 position);
        void Hide();
        void Cleanup();
    }
}