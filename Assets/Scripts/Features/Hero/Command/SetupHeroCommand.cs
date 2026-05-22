using Features.Hero.Model;
using QFramework;

namespace Features.Hero.Command
{
    public class SetupHeroCommand : AbstractCommand
    {
        private readonly HeroDefine mDefine;

        public SetupHeroCommand(HeroDefine define)
        {
            mDefine = define;
        }

        protected override void OnExecute()
        {
            IHeroModel model = this.GetModel<IHeroModel>();
            model.MaxHealth.Value = mDefine.MaxHealth;
            model.Health.Value = mDefine.MaxHealth;
            model.Invincible.Value = mDefine.Invincible;
        }
    }
}