using Features.Hero.Model;
using QFramework;

namespace Main.GM.Command
{
    public class AddHeroArmorCommand : AbstractCommand
    {
        private readonly int mAmount;

        public AddHeroArmorCommand(int amount)
        {
            mAmount = amount;
        }

        protected override void OnExecute()
        {
            IHeroModel heroModel = this.GetModel<IHeroModel>();
            heroModel.Armor.Value += mAmount;
        }
    }
}