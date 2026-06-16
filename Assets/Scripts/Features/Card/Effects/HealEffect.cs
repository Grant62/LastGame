using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class HealEffect : Effect
    {
        private readonly int mAmount;

        public HealEffect(int amount)
        {
            mAmount = amount;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            if (caster is IDamageable d)
                d.TakeHeal(mAmount);
        }
    }
}