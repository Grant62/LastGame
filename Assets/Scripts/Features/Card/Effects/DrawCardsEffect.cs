using Core.Architecture;
using Features.Card.System;
using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class DrawCardsEffect : Effect
    {
        private readonly int mCount;

        public DrawCardsEffect(int count)
        {
            mCount = count;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            GameMain.Interface.GetSystem<ICardSystem>().DrawCards(mCount);
        }
    }
}