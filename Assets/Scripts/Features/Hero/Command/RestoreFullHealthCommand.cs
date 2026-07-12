using Features.Hero.Model;
using QFramework;

namespace Features.Hero.Command
{
    public class RestoreFullHealthCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            IHeroModel hero = this.GetModel<IHeroModel>();
            hero.Health.Value = hero.MaxHealth.Value;
        }
    }
}