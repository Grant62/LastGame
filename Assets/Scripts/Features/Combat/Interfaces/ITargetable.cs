using UnityEngine;

namespace Features.Combat.Interfaces
{
    public interface ITargetable
    {
        Vector3 Position { get; }
        bool IsValidTarget { get; }
    }
}