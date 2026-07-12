using Features.Card.Data;
using Features.Combat.Interfaces;

namespace Features.Card.Effects
{
    public class ExhaustEffect : Effect
    {
        private readonly CardData mCardData;

        public ExhaustEffect(CardData cardData)
        {
            mCardData = cardData;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            Ctx.CardSystem.AddToConsume(mCardData);
        }
    }
}