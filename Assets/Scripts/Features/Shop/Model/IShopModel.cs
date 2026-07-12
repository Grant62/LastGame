using System.Collections.Generic;
using Features.Shop.Data;
using QFramework;

namespace Features.Shop.Model
{
    public interface IShopModel : IModel
    {
        List<ShopCardPackSlot> CardPackSlots { get; }
        EasyEvent OnShopChanged { get; }

        int RemoveCount { get; }
        int BaseRemoveCost { get; }
        int CostIncrement { get; }
        int CurrentRemovePrice { get; }

        void IncrementRemoveCount();
        void ResetRemoveCount();
        void SetRemoveCostConfig(int baseCost, int costIncrement);
    }
}