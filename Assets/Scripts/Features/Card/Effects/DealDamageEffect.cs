using Core.Architecture;
using Features.Combat.Targeting;
using Features.Combat.UI.Board;
using Features.Hero.Model;
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
            bool casterIsWeakened = false;
            IHeroModel heroModel = GameMain.Interface.GetModel<IHeroModel>();
            if (heroModel != null)
                casterIsWeakened = StatusHelper.HasStatus(heroModel.Statuses, StatusType.Weak);

            float damageMultiplier = casterIsWeakened ? 0.75f : 1f;

            foreach (ITargetable target in targets)
            {
                if (target is not IDamageable damageable || !damageable.IsValidTarget)
                    continue;

                float finalMultiplier = damageMultiplier;

                if (target is EnemyUI enemy && StatusHelper.HasStatus(enemy.Statuses, StatusType.Vulnerable))
                    finalMultiplier *= 1.25f;

                int finalDamage = Mathf.RoundToInt(mAmount * finalMultiplier);
                if (finalDamage > 0)
                    damageable.TakeDamage(finalDamage);
            }
        }
    }
}