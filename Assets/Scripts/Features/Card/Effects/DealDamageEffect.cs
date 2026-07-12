using Features.Combat.Define;
using Features.Combat.Interfaces;
using Features.Enemy.View;
using UnityEngine;

namespace Features.Card.Effects
{
    public class DealDamageEffect : Effect
    {
        private readonly int mAmount;

        public DealDamageEffect(int amount)
        {
            mAmount = amount;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            bool casterIsWeakened = StatusHelper.HasStatus(Ctx.HeroModel.Statuses, StatusType.Weak);

            float damageMultiplier = casterIsWeakened ? Ctx.Config.WeakMultiplier : 1f;

            foreach (ITargetable target in targets)
            {
                if (target is not IDamageable damageable || !damageable.IsValidTarget)
                    continue;

                float finalMultiplier = damageMultiplier;

                if (target is EnemyView enemy && StatusHelper.HasStatus(enemy.Statuses, StatusType.Vulnerable))
                    finalMultiplier *= Ctx.Config.VulnerableMultiplier;

                int finalDamage = Mathf.RoundToInt(mAmount * finalMultiplier);
                if (finalDamage > 0)
                    damageable.TakeDamage(finalDamage);
            }
        }
    }
}