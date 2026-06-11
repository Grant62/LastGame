using Features.Hero.Model;
using QFramework;

namespace Features.Hero.Command
{
    public class HeroGainArmorCommand : AbstractCommand
    {
        private readonly int mAmount;

        public HeroGainArmorCommand(int amount)
        {
            mAmount = amount;
        }

        protected override void OnExecute()
        {
            IHeroModel model = this.GetModel<IHeroModel>();
            model.Armor.Value += mAmount;
        }
    }
}