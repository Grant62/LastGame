using Features.Potion.Model;
using QFramework;

namespace Features.Potion.Command
{
    public class BuyPotionCommand : AbstractCommand
    {
        private readonly cfg.PotionInfo mPotion;

        public BuyPotionCommand(cfg.PotionInfo potion)
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