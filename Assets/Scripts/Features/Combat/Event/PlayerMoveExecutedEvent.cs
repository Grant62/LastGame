namespace Features.Combat.Event
{
    public struct PlayerMoveExecutedEvent
    {
        public int OldSlotIndex { get; }
        public int NewSlotIndex { get; }

        public PlayerMoveExecutedEvent(int oldSlotIndex, int newSlotIndex)
        {
            OldSlotIndex = oldSlotIndex;
            NewSlotIndex = newSlotIndex;
        }
    }
}