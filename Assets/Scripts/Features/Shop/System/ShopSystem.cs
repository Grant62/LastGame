using System;
using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Core.Systems;
using Features.Card.Data;
using Features.Card.Define;
using Features.Card.Model;
using Features.Shop.Data;
using Features.Shop.Model;
using QFramework;
using Services.ExcelTool;

namespace Features.Shop.System
{
    public class ShopSystem : AbstractSystem, IShopSystem
    {
        protected override void OnInit() { }

        public void GenerateShop()
        {
            IBinaryDataMgr mgr = this.GetUtility<IBinaryDataMgr>();
            ShopCardPackInfoContainer config = mgr.GetTable<ShopCardPackInfoContainer>();
            IShopModel model = this.GetModel<IShopModel>();
            model.CardPackSlots.Clear();

            foreach (ShopCardPackInfo info in config.DataDic.Values)
            {
                model.CardPackSlots.Add(new ShopCardPackSlot
                {
                    Name = info.Name,
                    Desc = info.Desc,
                    Price = info.Price,
                    IsSold = false,
                    TotalOptions = info.TotalOptions,
                    PickCount = info.PickCount,
                    RarityFilter = info.RarityFilter,
                    Address = info.Address
                });
            }

            RemoveCardCostInfoContainer removeConfig = mgr.GetTable<RemoveCardCostInfoContainer>();
            if (removeConfig != null && removeConfig.DataDic.Count > 0)
            {
                RemoveCardCostInfo removeInfo = removeConfig.DataDic[1];
                model.SetRemoveCostConfig(removeInfo.Cost, removeInfo.Increment);
            }

            model.ResetRemoveCount();
            model.OnShopChanged.Trigger();
        }

        public List<CardData> GenerateCandidates(int slotIndex)
        {
            IShopModel model = this.GetModel<IShopModel>();
            ShopCardPackSlot slot = model.CardPackSlots[slotIndex];
            ICardDefineModel defineModel = this.GetModel<ICardDefineModel>();

            List<CardDefine> filtered = new();
            foreach (string rarity in slot.RarityFilter.Split(','))
                filtered.AddRange(defineModel.GetDefinesByRarity(rarity.Trim()));

            Shuffle(filtered);

            int count = Math.Min(slot.TotalOptions, filtered.Count);
            List<CardData> candidates = new(count);
            for (int i = 0; i < count; i++)
                candidates.Add(filtered[i].CreateCardData());

            return candidates;
        }

        public void MarkSold(int slotIndex)
        {
            IShopModel model = this.GetModel<IShopModel>();
            model.CardPackSlots[slotIndex].IsSold = true;
            model.OnShopChanged.Trigger();
        }

        private void Shuffle<T>(IList<T> list)
        {
            IRandomSystem random = this.GetSystem<IRandomSystem>();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Range(0, i + 1, RandomModuleIds.Merchant);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}