namespace Features.Combat.Event
{
    public struct EnemyDiedEvent
    {
        public int SlotIndex { get; }

        public EnemyDiedEvent(int slotIndex)
        {
            SlotIndex = slotIndex;
        }
    }
}