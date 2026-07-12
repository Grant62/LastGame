using Features.Combat.Interfaces;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class SpinSwordEffect : Effect
    {
        private readonly int mDamagePerSpin;

        public SpinSwordEffect(int damagePerSpin = 0)
        {
            mDamagePerSpin = damagePerSpin;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = Ctx.SwordModel;
            int dmg = mDamagePerSpin > 0 ? mDamagePerSpin : Ctx.Config.SpinBaseDamage;

            model.IsSpinning.Value = true;
            model.SpinDamage.Value += dmg;
        }
    }
}