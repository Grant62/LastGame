using Core.Architecture;
using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class StopSpinEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = GameMain.Interface.GetModel<ISwordModel>();
            model.IsSpinning.Value = false;
            model.SpinDamage.Value = 0;
        }
    }
}