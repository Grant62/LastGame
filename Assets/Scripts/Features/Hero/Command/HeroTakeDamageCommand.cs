using Features.Hero.Event;
using Features.Hero.Model;
using QFramework;
using UnityEngine;

namespace Features.Hero.Command
{
    public class HeroTakeDamageCommand : AbstractCommand
    {
        private readonly int mAmount;

        public HeroTakeDamageCommand(int amount)
        {
            mAmount = amount;
        }

        protected override void OnExecute()
        {
            IHeroModel model = this.GetModel<IHeroModel>();

            if (mAmount <= 0)
                return;

            if (model.Invincible.Value)
                return;

            model.Health.Value -= mAmount;
            model.Health.Value = Mathf.Max(0, model.Health.Value);

            if (model.Health.Value <= 0)
                this.SendEvent<HeroDeathEvent>();
        }
    }
}