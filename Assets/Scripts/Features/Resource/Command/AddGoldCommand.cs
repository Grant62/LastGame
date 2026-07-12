using Features.Resource.Model;
using QFramework;

namespace Features.Resource.Command
{
    public class AddGoldCommand : AbstractCommand
    {
        private readonly int mAmount;

        public AddGoldCommand(int amount)
        {
            mAmount = amount;
        }

        protected override void OnExecute()
        {
            IResourceModel model = this.GetModel<IResourceModel>();
            model.Gold.Value += mAmount;
        }
    }
}