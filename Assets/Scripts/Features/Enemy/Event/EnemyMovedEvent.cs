namespace Features.Enemy.Event
{
    public struct EnemyMovedEvent
    {
        public int FromSlotIndex;

        public int NewSlotIndex;

        public EnemyMovedEvent(int fromSlot, int toSlot)
        {
            FromSlotIndex = fromSlot;
            NewSlotIndex = toSlot;
        }
    }
}