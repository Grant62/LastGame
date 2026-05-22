using QFramework;
using UnityEngine;

namespace Features.Combat.Targeting
{
    public interface ITargetSelector : IUtility
    {
        ITargetable GetTargetAtPosition(Vector3 position);
        ITargetable GetTargetAtMousePosition();
    }
}