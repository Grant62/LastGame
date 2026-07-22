using System.Collections.Generic;
using Features.Potion.Data;
using Features.Shop.Data;
using QFramework;

namespace Features.Shop.Model
{
    public class ShopModel : AbstractModel, IShopModel
    {
        public List<ShopCardPackSlot> CardPackSlots { get; } = new();
        public List<ShopPotionSlot> PotionShopSlots { get; } = new();
        public EasyEvent OnShopChanged { get; } = new();

        public int RemoveCount { get; private set; }
        public int BaseRemoveCost { get; private set; }
        public int CostIncrement { get; private set; }

        public int CurrentRemovePrice { get => BaseRemoveCost + RemoveCount * CostIncrement; }

        public void IncrementRemoveCount()
        {
            RemoveCount++;
            OnShopChanged.Trigger();
        }

        public void ResetRemoveCount()
        {
            RemoveCount = 0;
        }

        public void SetRemoveCostConfig(int baseCost, int costIncrement)
        {
            BaseRemoveCost = baseCost;
            CostIncrement = costIncrement;
        }

        protected override void OnInit() { }
    }
}