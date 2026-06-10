using Core.Architecture;
using Features.Card.Data;
using Features.Card.Model;
using Features.Combat.Targeting;

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
            ICardModel model = GameMain.Interface.GetModel<ICardModel>();
            model.HandPile.Remove(mCardData);
            model.DrawPile.Remove(mCardData);
            model.DiscardPile.Remove(mCardData);
        }
    }
}