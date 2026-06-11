using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class GainEnergyEffect : Effect
    {
        private readonly int mAmount;

        public GainEnergyEffect(int amount)
        {
            mAmount = amount;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            Ctx.ResourceSystem.Gain(mAmount);
        }
    }
}