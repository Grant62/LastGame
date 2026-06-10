using Core.Architecture;
using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class SpinSwordEffect : Effect
    {
        private readonly int mDamagePerSpin;

        public SpinSwordEffect(int damagePerSpin = 3)
        {
            mDamagePerSpin = damagePerSpin;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = GameMain.Interface.GetModel<ISwordModel>();

            model.IsSpinning.Value = true;
            model.SpinDamage.Value += mDamagePerSpin;
        }
    }
}