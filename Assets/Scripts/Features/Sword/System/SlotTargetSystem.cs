using Features.Hero.Model;
using Features.Sword.Model;
using QFramework;
using Services;
using UnityEngine;

namespace Features.Sword.System
{
    public class SlotTargetSystem : AbstractSystem, ISlotTargetSystem
    {
        protected override void OnInit() { }

        public bool Validate(int cardId, string desc, int targetSlotIndex)
        {
            int distance = CardDescriptionParser.ParseDistance(desc);

            if (cardId == 12002)
            {
                ISwordModel sword = this.GetModel<ISwordModel>();
                if (!sword.IsSummoned.Value)
                    return false;

                return Mathf.Abs(sword.CurrentSlotIndex.Value - targetSlotIndex) <= distance;
            }

            if (cardId == 12003)
            {
                IHeroModel hero = this.GetModel<IHeroModel>();
                return Mathf.Abs(hero.CurrentSlotIndex.Value - targetSlotIndex) <= distance;
            }

            return false;
        }
    }
}