namespace Features.Card.Utility
{
    public struct EffectSlot
    {
        public EffectType Type;
        public EffectTarget Target;
        public string Param1;
        public string Param2;
        public EffectCondition Condition;

        public bool IsEmpty { get => Type == EffectType.None; }
    }
}