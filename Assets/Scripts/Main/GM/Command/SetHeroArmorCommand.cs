using Features.Hero.Model;
using QFramework;

namespace Main.GM.Command
{
    public class SetHeroArmorCommand : AbstractCommand
    {
        private readonly int mArmor;

        public SetHeroArmorCommand(int armor)
        {
            mArmor = armor;
        }

        protected override void OnExecute()
        {
            this.GetModel<IHeroModel>().Armor.Value = mArmor;
        }
    }
}