using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class ReduceSpinCardCostEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            Ctx.CardSystem.ReduceSpinCardCosts();
        }
    }
}