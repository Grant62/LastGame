using Features.Hero.Model;
using QFramework;

namespace Features.Hero.Command
{
    public class SetHeroSlotCommand : AbstractCommand
    {
        private readonly int mSlotIndex;

        public SetHeroSlotCommand(int slotIndex)
        {
            mSlotIndex = slotIndex;
        }

        protected override void OnExecute()
        {
            this.GetModel<IHeroModel>().CurrentSlotIndex.Value = mSlotIndex;
        }
    }
}