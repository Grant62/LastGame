using Features.Combat.Interfaces;

namespace Features.Card.Effects
{
    public class SwordPathDamageEffect : Effect
    {
        private readonly int mDamage;

        public SwordPathDamageEffect(int damage = 4)
        {
            mDamage = damage;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            foreach (ITargetable target in targets)
            {
                if (target is IDamageable d && d.IsValidTarget)
                    d.TakeDamage(mDamage);
            }
        }
    }
}