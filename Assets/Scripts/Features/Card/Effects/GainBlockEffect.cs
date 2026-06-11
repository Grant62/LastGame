using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class GainBlockEffect : Effect
    {
        private readonly int mAmount;

        public GainBlockEffect(int amount)
        {
            mAmount = amount;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            foreach (ITargetable target in targets)
            {
                if (target is IDamageable damageable)
                    damageable.GainArmor(mAmount);
            }
        }
    }
}