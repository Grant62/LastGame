using Features.Hero.Model;
using QFramework;

namespace Features.Hero.Command
{
    public class SetFacingCommand : AbstractCommand
    {
        private readonly bool mFacingRight;

        public SetFacingCommand(bool facingRight)
        {
            mFacingRight = facingRight;
        }

        protected override void OnExecute()
        {
            this.GetModel<IHeroModel>().IsFacingRight.Value = mFacingRight;
        }
    }
}