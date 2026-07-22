using Configuration.ExcelData.DataClass;
using Features.Potion.Model;
using QFramework;

namespace Features.Potion.Command
{
    public class BuyPotionCommand : AbstractCommand
    {
        private readonly PotionInfo mPotion;

        public BuyPotionCommand(PotionInfo potion)
        {
            mPotion = potion;
        }

        protected override void OnExecute()
        {
            IPotionModel model = this.GetModel<IPotionModel>();
            if (model.IsFull)
                return;

            model.AddPotion(mPotion);
        }
    }
}