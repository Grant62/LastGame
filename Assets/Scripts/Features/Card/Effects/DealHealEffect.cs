using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class DealHealEffect : Effect
    {
        private readonly int mAmount;

        public DealHealEffect(int amount)
        {
            mAmount = amount;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            foreach (ITargetable target in targets)
            {
                if (target is IDamageable damagable && damagable.IsValidTarget)
                    damagable.TakeHeal(mAmount);
            }
        }
    }
}