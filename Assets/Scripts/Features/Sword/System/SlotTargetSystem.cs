using Features.Card.Data;
using Features.Hero.Model;
using Features.Sword.Model;
using QFramework;
using UnityEngine;

namespace Features.Sword.System
{
    public class SlotTargetSystem : AbstractSystem, ISlotTargetSystem
    {
        protected override void OnInit() { }

        public bool Validate(CardData cardData, int targetSlotIndex)
        {
            if (cardData.SlotAction == SlotAction.MoveSword)
            {
                ISwordModel sword = this.GetModel<ISwordModel>();

                return true;
            }

            if (cardData.SlotAction == SlotAction.MovePlayer)
            {
                IHeroModel hero = this.GetModel<IHeroModel>();
                int distance = cardData.SlotDistance;
                if (distance <= 0)
                    return true;

                return Mathf.Abs(hero.CurSlotIndex.Value - targetSlotIndex) <= distance;
            }

            return false;
        }
    }
}