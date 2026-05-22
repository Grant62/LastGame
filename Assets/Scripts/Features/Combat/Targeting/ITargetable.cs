using UnityEngine;

namespace Features.Combat.Targeting
{
    public interface ITargetable
    {
        Vector3 Position { get; }
        bool IsValidTarget { get; }
    }
}