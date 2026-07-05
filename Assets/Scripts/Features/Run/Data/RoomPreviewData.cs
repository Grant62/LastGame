namespace Features.Run.Data
{
    public enum RoomBoxState
    {
        Upcoming,
        Current,
        Rested,
        Cleared
    }

    public readonly struct RoomPreviewData
    {
        public readonly int Layer;
        public readonly int Step;
        public readonly string StepTypeText;
        public readonly RoomBoxState State;
        public readonly bool CanShortRest;
        public readonly int ShortRestCount;
        public readonly string BossPreview;

        public RoomPreviewData(int layer, int step, string stepTypeText, RoomBoxState state,
            bool canShortRest, int shortRestCount, string bossPreview = "")
        {
            Layer = layer;
            Step = step;
            StepTypeText = stepTypeText;
            State = state;
            CanShortRest = canShortRest;
            ShortRestCount = shortRestCount;
            BossPreview = bossPreview;
        }
    }
}