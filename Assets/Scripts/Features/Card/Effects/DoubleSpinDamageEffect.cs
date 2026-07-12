using Features.Combat.Interfaces;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class DoubleSpinDamageEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = Ctx.SwordModel;
            model.SpinDamage.Value *= 2;
        }
    }
}