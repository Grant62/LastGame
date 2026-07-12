using Features.Combat.Interfaces;

namespace Features.Card.Effects
{
    public class RestoreEnergyToMaxEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            Ctx.ResourceModel.CurEnergy.Value = Ctx.ResourceModel.MaxEnergy.Value;
        }
    }
}