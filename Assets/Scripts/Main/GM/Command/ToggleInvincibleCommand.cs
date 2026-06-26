using Features.Hero.Model;
using QFramework;

namespace Main.GM.Command
{
    public class ToggleInvincibleCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            IHeroModel model = this.GetModel<IHeroModel>();
            model.Invincible.Value = !model.Invincible.Value;
        }
    }
}