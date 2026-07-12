using Features.Hero.Model;
using QFramework;

namespace Features.Hero.Command
{
    public class SetInvincibleCommand : AbstractCommand
    {
        private readonly bool mValue;

        public SetInvincibleCommand(bool value)
        {
            mValue = value;
        }

        protected override void OnExecute()
        {
            this.GetModel<IHeroModel>().Invincible.Value = mValue;
        }
    }
}