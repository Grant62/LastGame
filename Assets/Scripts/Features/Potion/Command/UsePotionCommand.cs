using Configuration.ExcelData.DataClass;
using Features.Combat.Interfaces;
using Features.Hero.Model;
using Features.Potion.Model;
using Features.Resource.Model;
using QFramework;
using UnityEngine;

namespace Features.Potion.Command
{
    public class UsePotionCommand : AbstractCommand
    {
        private readonly int mSlotIndex;
        private readonly ITargetable mTarget;

        public UsePotionCommand(int slotIndex, ITargetable target = null)
        {
            mSlotIndex = slotIndex;
            mTarget = target;
        }

        protected override void OnExecute()
        {
            IPotionModel model = this.GetModel<IPotionModel>();
            PotionInfo potion = model.GetPotionAt(mSlotIndex);
            if (potion == null)
                return;

            switch (potion.EffectType)
            {
                case "Heal":
                    IHeroModel hero = this.GetModel<IHeroModel>();
                    int healAmount = potion.EffectValue;
                    hero.Health.Value = Mathf.Min(
                        hero.Health.Value + healAmount, hero.MaxHealth.Value);
                    break;

                case "Energy":
                    this.GetModel<IResourceModel>().CurEnergy.Value += potion.EffectValue;
                    break;

                case "Block":
                    if (mTarget is IDamageable damageable)
                        damageable.GainArmor(potion.EffectValue);
                    break;

                case "Damage":
                    if (mTarget is IDamageable target)
                        target.TakeDamage(potion.EffectValue);
                    break;
            }

            model.RemoveAt(mSlotIndex);
        }
    }
}