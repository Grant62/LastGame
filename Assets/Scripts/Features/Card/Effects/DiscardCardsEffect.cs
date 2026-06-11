using Core.Systems;
using Features.Card.Model;
using Features.Card.System;
using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class DiscardCardsEffect : Effect
    {
        private readonly int mCount;

        public DiscardCardsEffect(int count)
        {
            mCount = count;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ICardModel model = Ctx.CardModel;
            ICardSystem system = Ctx.CardSystem;

            for (int i = 0; i < mCount && model.HandPile.Count > 0; i++)
            {
                int index = Ctx.RandomSystem.Range(0, model.HandPile.Count, RandomModuleIds.Combat);
                system.DiscardFromHand(model.HandPile[index]);
            }
        }
    }
}