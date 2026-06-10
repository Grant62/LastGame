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

            if (model.IsSpinning.Value && !model.KeepSpinningOnMove)
            {
                model.IsSpinning.Value = false;
                model.SpinDamage.Value = 0;
            }

            model.KeepSpinningOnMove = false;
            model.CurSlotIndex.Value = mTargetSlotIndex;
        }
    }
}