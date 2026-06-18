using Features.Card.Data;
using Features.Card.Define;
using Features.Card.Model;
using Features.Card.System;
using QFramework;

namespace Main.GM.Command
{
    public class GiveCardCommand : AbstractCommand
    {
        private readonly int mCardId;
        private readonly int mCount;
        private readonly string mTargetPile;

        public GiveCardCommand(int cardId, int count, string targetPile)
        {
            mCardId = cardId;
            mCount = count;
            mTargetPile = targetPile;
        }

        protected override void OnExecute()
        {
            ICardDefineModel defineModel = this.GetModel<ICardDefineModel>();
            if (!defineModel.TryGet(mCardId, out CardDefine define))
                return;

            ICardSystem cardSystem = this.GetSystem<ICardSystem>();
            ICardModel cardModel = this.GetModel<ICardModel>();

            for (int i = 0; i < mCount; i++)
            {
                CardData cardData = define.CreateCardData();
                switch (mTargetPile)
                {
                    case "hand":
                        cardSystem.AddToHand(cardData);
                        break;
                    case "discard":
                        cardSystem.AddToDiscard(cardData);
                        break;
                    case "draw":
                    case "deck":
                        cardModel.DrawPile.Add(cardData);
                        cardModel.OnDrawPileChanged.Trigger();
                        break;
                }
            }
        }
    }
}