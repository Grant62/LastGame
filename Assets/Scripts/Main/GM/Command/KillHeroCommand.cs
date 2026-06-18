using Features.Hero.Model;
using QFramework;

namespace Main.GM.Command
{
    public class KillHeroCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<IHeroModel>().Health.Value = 0;
        }
    }
}