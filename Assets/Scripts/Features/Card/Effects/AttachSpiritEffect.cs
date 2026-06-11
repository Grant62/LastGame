using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class AttachSpiritEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = Ctx.SwordModel;
            model.IsSpiritAttached = true;
        }
    }
}