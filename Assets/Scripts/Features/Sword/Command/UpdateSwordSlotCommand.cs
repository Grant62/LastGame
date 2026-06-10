using Features.Sword.Model;
using QFramework;

namespace Features.Sword.Command
{
    public class UpdateSwordSlotCommand : AbstractCommand
    {
        private readonly int mSlotIndex;

        public UpdateSwordSlotCommand(int slotIndex)
        {
            mSlotIndex = slotIndex;
        }

        protected override void OnExecute()
        {
            ISwordModel sword = this.GetModel<ISwordModel>();
            sword.CurSlotIndex.Value = mSlotIndex;
        }
    }
}