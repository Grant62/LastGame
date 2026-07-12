using Features.Card.Data;
using Features.Card.System;
using Features.Shop.Model;
using QFramework;

namespace Features.Shop.Command
{
    public class RemoveCardFromLibraryCommand : AbstractCommand
    {
        private readonly CardData mCardData;

        public RemoveCardFromLibraryCommand(CardData cardData)
        {
            mCardData = cardData;
        }

        protected override void OnExecute()
        {
            this.GetSystem<ICardSystem>().RemoveFromLibrary(mCardData);
            this.GetModel<IShopModel>().IncrementRemoveCount();
        }
    }
}