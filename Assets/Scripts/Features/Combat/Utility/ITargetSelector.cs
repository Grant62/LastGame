using Features.Combat.Targeting;
using QFramework;
using UnityEngine;

namespace Features.Combat.Utility
{
    public interface ITargetSelector : IUtility
    {
        ITargetable GetTargetAtPosition(Vector3 position);
        ITargetable GetTargetAtMousePosition();
        ITargetable GetCaster();
    }
}