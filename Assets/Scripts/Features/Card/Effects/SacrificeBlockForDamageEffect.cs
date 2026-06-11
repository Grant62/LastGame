using Features.Combat.Targeting;
using Features.Hero.Model;
using UnityEngine;

namespace Features.Card.Effects
{
    public class SacrificeBlockForDamageEffect : Effect
    {
        private readonly float mRecoveryRatio;

        public SacrificeBlockForDamageEffect(float recoveryRatio = 0.5f)
        {
            mRecoveryRatio = recoveryRatio;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            IHeroModel hero = Ctx.HeroModel;
            int armorAmount = hero.Armor.Value;

            if (armorAmount <= 0)
                return;

            hero.Armor.Value = 0;

            foreach (ITargetable target in targets)
            {
                if (target is IDamageable d && d.IsValidTarget)
                    d.TakeDamage(armorAmount);
            }

            int restore = Mathf.RoundToInt(armorAmount * mRecoveryRatio);
            if (restore > 0 && caster is IDamageable cd)
                cd.GainArmor(restore);
        }
    }
}