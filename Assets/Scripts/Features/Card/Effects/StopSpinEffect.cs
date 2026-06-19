using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class StopSpinEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;
            sword.IsSpinning.Value = false;
            sword.SpinDamage.Value = 0;
        }
    }
}