using Features.Resource.Model;
using QFramework;
using UnityEngine;

namespace Features.Resource.Command
{
    public class SpendGoldCommand : AbstractCommand
    {
        private readonly int mAmount;

        public SpendGoldCommand(int amount)
        {
            mAmount = amount;
        }

        protected override void OnExecute()
        {
            IResourceModel model = this.GetModel<IResourceModel>();
            model.Gold.Value = Mathf.Max(0, model.Gold.Value - mAmount);
        }
    }
}