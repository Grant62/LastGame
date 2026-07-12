using Features.Combat.Define;

namespace Features.Card.Effects
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