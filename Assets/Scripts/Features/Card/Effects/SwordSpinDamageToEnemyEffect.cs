using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class SwordSpinDamageToEnemyEffect : Effect
    {
        private readonly bool mStopAfter;

        public SwordSpinDamageToEnemyEffect(bool stopAfter = true)
        {
            mStopAfter = stopAfter;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;
            int damage = sword.SpinDamage.Value;
            if (damage <= 0)
                return;

            foreach (ITargetable target in targets)
            {
                if (target is IDamageable { IsValidTarget: true } damageable)
                    damageable.TakeDamage(damage);
            }

            if (mStopAfter)
            {
                sword.IsSpinning.Value = false;
                sword.SpinDamage.Value = 0;
            }
        }
    }
}