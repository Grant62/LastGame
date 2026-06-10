using Core.Architecture;
using Features.Combat.Targeting;
using Features.Resource.System;

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
            GameMain.Interface.GetSystem<IResourceSystem>().Gain(mAmount);
        }
    }
}