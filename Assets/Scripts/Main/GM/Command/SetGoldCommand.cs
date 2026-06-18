using Features.Resource.Model;
using QFramework;

namespace Main.GM.Command
{
    public class SetGoldCommand : AbstractCommand
    {
        private readonly int mGold;

        public SetGoldCommand(int gold)
        {
            mGold = gold;
        }

        protected override void OnExecute()
        {
            this.GetModel<IResourceModel>().Gold.Value = mGold;
        }
    }
}