using Features.Sword.Model;
using QFramework;

namespace Features.Sword.Command
{
    public class MoveSwordCommand : AbstractCommand
    {
        private readonly int mTargetSlotIndex;

        public MoveSwordCommand(int targetSlotIndex)
        {
            mTargetSlotIndex = targetSlotIndex;
        }

        protected override void OnExecute()
        {
            ISwordModel model = this.GetModel<ISwordModel>();
            if (!model.IsSummoned.Value)
                return;

            model.CurrentSlotIndex.Value = mTargetSlotIndex;
            model.IsFollowingPlayer.Value = false;
        }
    }
}