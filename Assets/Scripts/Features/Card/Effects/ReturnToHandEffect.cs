using Features.Card.Data;
using Features.Combat.Interfaces;

namespace Features.Card.Effects
{
    public class ReturnToHandEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            CardData card = Ctx.PlayedCard;
            if (card == null)
                return;

            Ctx.CardSystem.ReturnToHand(card);
        }
    }
}