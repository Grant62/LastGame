using UnityEngine;

namespace Features.Combat.Targeting.Event
{
    public readonly struct TargetingStartedEvent
    {
        public Vector3 StartPosition { get; }

        public TargetingStartedEvent(Vector3 startPosition)
        {
            StartPosition = startPosition;
        }
    }

    public readonly struct TargetingEndedEvent { }
}