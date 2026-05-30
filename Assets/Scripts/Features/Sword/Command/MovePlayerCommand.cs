using Features.Combat.Event;
using Features.Hero.Model;
using Features.Sword.Model;
using QFramework;

namespace Features.Sword.Command
{
    public class MovePlayerCommand : AbstractCommand
    {
        private readonly int mTargetSlotIndex;

        public MovePlayerCommand(int targetSlotIndex)
        {
            mTargetSlotIndex = targetSlotIndex;
        }

        protected override void OnExecute()
        {
            IHeroModel heroModel = this.GetModel<IHeroModel>();
            int oldSlotIndex = heroModel.CurrentSlotIndex.Value;

            if (mTargetSlotIndex == oldSlotIndex)
                return;

            heroModel.CurrentSlotIndex.Value = mTargetSlotIndex;

            ISwordModel swordModel = this.GetModel<ISwordModel>();
            if (swordModel.IsSummoned.Value && swordModel.CurrentSlotIndex.Value == oldSlotIndex)
                swordModel.CurrentSlotIndex.Value = mTargetSlotIndex;

            this.SendEvent(new PlayerMoveExecutedEvent(oldSlotIndex, mTargetSlotIndex));
        }
    }
}