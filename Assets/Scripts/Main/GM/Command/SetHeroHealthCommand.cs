using Features.Hero.Model;
using QFramework;

namespace Main.GM.Command
{
    public class SetHeroHealthCommand : AbstractCommand
    {
        private readonly int mHealth;

        public SetHeroHealthCommand(int health)
        {
            mHealth = health;
        }

        protected override void OnExecute()
        {
            IHeroModel heroModel = this.GetModel<IHeroModel>();
            heroModel.Health.Value = mHealth;
            heroModel.MaxHealth.Value = mHealth;
        }
    }
}