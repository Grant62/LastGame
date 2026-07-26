using System;
using System.Collections.Generic;
using Core.Systems;
using Features.Card.Data;
using Features.Card.Define;
using Features.Card.Model;
using Features.Configuration.Model;
using Features.Potion.Data;
using Features.Shop.Data;
using Features.Shop.Model;
using QFramework;

namespace Features.Shop.System
{
    public class ShopSystem : AbstractSystem, IShopSystem
    {
        protected override void OnInit() { }

        public void GenerateShop()
        {
            cfg.Tables tables = this.GetUtility<ILubanDataModel>().Tables;
            IShopModel model = this.GetModel<IShopModel>();
            model.CardPackSlots.Clear();

            foreach (cfg.ShopCardPackInfo info in tables.TbShopCardPackInfo.DataList)
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

            var removeList = tables.TbRemoveCardCostInfo.DataList;
            if (removeList.Count > 0)
            {
                cfg.RemoveCardCostInfo removeInfo = removeList[0];
                model.SetRemoveCostConfig(removeInfo.Cost, removeInfo.Increment);
            }

            model.ResetRemoveCount();

            model.PotionShopSlots.Clear();
            var allPotions = new List<cfg.PotionInfo>(tables.TbPotionInfo.DataList);
            if (allPotions.Count > 0)
            {
                IRandomSystem random = this.GetSystem<IRandomSystem>();
                int count = Math.Min(3, allPotions.Count);
                for (int i = 0; i < count; i++)
                {
                    int index = random.Range(0, allPotions.Count, RandomModuleIds.Combat);
                    model.PotionShopSlots.Add(new ShopPotionSlot { Info = allPotions[index] });
                    allPotions.RemoveAt(index);
                }
            }

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