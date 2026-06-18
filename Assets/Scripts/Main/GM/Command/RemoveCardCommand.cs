using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Model;
using Features.Card.System;
using QFramework;

namespace Main.GM.Command
{
    public class RemoveCardCommand : AbstractCommand
    {
        private readonly int mCardId;
        private readonly string mTargetPile;

        public RemoveCardCommand(int cardId, string targetPile)
        {
            mCardId = cardId;
            mTargetPile = targetPile;
        }

        protected override void OnExecute()
        {
            ICardModel cardModel = this.GetModel<ICardModel>();
            ICardSystem cardSystem = this.GetSystem<ICardSystem>();

            List<CardData> pile = mTargetPile switch
            {
                "hand" => cardModel.HandPile,
                "draw" or "deck" => cardModel.DrawPile,
                "discard" => cardModel.DiscardPile,
                _ => null
            };

            if (pile == null)
                return;

            CardData found = pile.Find(c => c.CardId == mCardId);
            if (found == null)
                return;

            if (mTargetPile == "hand")
            {
                cardSystem.RemoveFromHand(found);
            }
            else
            {
                pile.Remove(found);
                switch (mTargetPile)
                {
                    case "draw":
                    case "deck":
                        cardModel.OnDrawPileChanged.Trigger();
                        break;
                    case "discard":
                        cardModel.OnDiscardPileChanged.Trigger();
                        break;
                }
            }
        }
    }
}