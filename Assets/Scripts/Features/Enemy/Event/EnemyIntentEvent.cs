using Features.Enemy.Define;

namespace Features.Enemy.Event
{
    public struct EnemyIntentEvent
    {
        public int SlotIndex;

        public EnemyIntentType Intent;
    }
}