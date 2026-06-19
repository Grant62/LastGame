using Features.Card.Data;
using Features.Card.Model;
using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class ReturnToHandEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            CardData card = Ctx.PlayedCard;
            if (card == null)
                return;

            ICardModel model = Ctx.CardModel;
            model.DiscardPile.Remove(card);
            model.OnDiscardPileChanged.Trigger();
            model.HandPile.Add(card);
            model.OnHandPileChanged.Trigger();
        }
    }
}