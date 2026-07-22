using System;
using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Core.Systems;
using Features.Combat.Interfaces;
using Features.Hero.Model;
using Features.Potion.Model;
using Features.Resource.Model;
using QFramework;
using Services.ExcelTool;
using UnityEngine;

namespace Features.Potion.System
{
    public class PotionSystem : AbstractSystem, IPotionSystem
    {
        private PotionInfoContainer mPotionTable;

        protected override void OnInit()
        {
            mPotionTable = this.GetUtility<IBinaryDataMgr>().GetTable<PotionInfoContainer>();
        }

        public List<PotionInfo> GenerateShopPotions()
        {
            List<PotionInfo> result = new();
            if (mPotionTable?.DataDic == null || mPotionTable.DataDic.Count == 0)
                return result;

            List<PotionInfo> allPotions = new(mPotionTable.DataDic.Values);
            IRandomSystem random = this.GetSystem<IRandomSystem>();

            int count = Math.Min(3, allPotions.Count);
            for (int i = 0; i < count; i++)
            {
                int index = random.Range(0, allPotions.Count, RandomModuleIds.Combat);
                result.Add(allPotions[index]);
                allPotions.RemoveAt(index);
            }

            return result;
        }

        public void UsePotion(int slotIndex, ITargetable target = null)
        {
            IPotionModel model = this.GetModel<IPotionModel>();
            PotionInfo potion = model.GetPotionAt(slotIndex);
            if (potion == null)
                return;

            switch (potion.EffectType)
            {
                case "Heal":
                {
                    IHeroModel hero = this.GetModel<IHeroModel>();
                    int healAmount = potion.EffectValue;
                    hero.Health.Value = Mathf.Min(
                        hero.Health.Value + healAmount, hero.MaxHealth.Value);
                    break;
                }
                case "Energy":
                    this.GetModel<IResourceModel>().CurEnergy.Value += potion.EffectValue;
                    break;
                case "Block":
                    if (target is IDamageable damageable)
                        damageable.GainArmor(potion.EffectValue);
                    break;
                case "Damage":
                    if (target is IDamageable d)
                        d.TakeDamage(potion.EffectValue);
                    break;
            }

            model.RemoveAt(slotIndex);
        }
    }
}