using Features.Hero.Model;
using QFramework;
using UnityEngine;

namespace Features.Hero.Command
{
    public class HeroTakeHealCommand : AbstractCommand
    {
        private readonly int mAmount;

        public HeroTakeHealCommand(int amount)
        {
            mAmount = amount;
        }

        protected override void OnExecute()
        {
            IHeroModel model = this.GetModel<IHeroModel>();

            if (mAmount <= 0)
                return;

            model.Health.Value += mAmount;
            model.Health.Value = Mathf.Min(model.MaxHealth.Value, model.Health.Value);
        }
    }
}