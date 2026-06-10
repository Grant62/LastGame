using Features.Combat.Event;
using Features.Hero.Model;
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
            int oldSlotIndex = heroModel.CurSlotIndex.Value;

            if (mTargetSlotIndex == oldSlotIndex)
                return;

            heroModel.CurSlotIndex.Value = mTargetSlotIndex;

            this.SendEvent(new PlayerMoveExecutedEvent(oldSlotIndex, mTargetSlotIndex));
        }
    }
}