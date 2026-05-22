namespace Features.Combat.Targeting
{
    public class AutoTargetEffect
    {
        public TargetType TargetType { get; }
        public Effect Effect { get; }

        public AutoTargetEffect(TargetType targetType, Effect effect)
        {
            TargetType = targetType;
            Effect = effect;
        }
    }
}