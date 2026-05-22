namespace Features.Resource.Event
{
    public readonly struct ResourceChangedEvent
    {
        public int Previous { get; }
        public int Current { get; }

        public ResourceChangedEvent(int previous, int current)
        {
            Previous = previous;
            Current = current;
        }
    }
}