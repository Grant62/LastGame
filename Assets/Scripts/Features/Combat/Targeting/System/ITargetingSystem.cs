using QFramework;
using UnityEngine;

namespace Features.Combat.Targeting.System
{
    public interface ITargetingSystem : ISystem
    {
        ITargetable GetTargetAtPosition(Vector3 position);
        ITargetable GetTargetAtMousePosition();
        ITargetable[] ResolveTargets(TargetType type, ITargetable caster);
    }
}