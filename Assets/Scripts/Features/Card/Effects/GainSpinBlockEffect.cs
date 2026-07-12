using Features.Combat.Interfaces;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class GainSpinBlockEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;
            int damage = sword.SpinDamage.Value;
            if (damage > 0 && caster is IDamageable damageable)
                damageable.GainArmor(damage);
        }
    }
}