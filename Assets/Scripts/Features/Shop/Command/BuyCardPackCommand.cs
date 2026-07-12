using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.System;
using Features.Shop.Data;
using Features.Shop.Model;
using Features.Shop.System;
using QFramework;

namespace Features.Shop.Command
{
    public class BuyCardPackCommand : AbstractCommand
    {
        private readonly int mSlotIndex;
        private readonly List<CardData> mSelectedCards;

        public BuyCardPackCommand(int slotIndex, List<CardData> selectedCards)
        {
            mSlotIndex = slotIndex;
            mSelectedCards = selectedCards;
        }

        protected override void OnExecute()
        {
            IShopModel shopModel = this.GetModel<IShopModel>();
            ShopCardPackSlot slot = shopModel.CardPackSlots[mSlotIndex];

            if (slot.IsSold)
                return;

            ICardSystem cardSystem = this.GetSystem<ICardSystem>();
            foreach (CardData card in mSelectedCards)
                cardSystem.AddToLibrary(card);

            this.GetSystem<IShopSystem>().MarkSold(mSlotIndex);
        }
    }
}