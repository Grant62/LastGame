using Features.Combat.Event;
using Features.Combat.System;
using QFramework;

namespace Features.Combat.Command
{
    public class SendEnemyDiedCommand : AbstractCommand
    {
        private readonly int mSlotIndex;

        public SendEnemyDiedCommand(int slotIndex)
        {
            mSlotIndex = slotIndex;
        }

        protected override void OnExecute()
        {
            this.SendEvent(new EnemyDiedEvent(mSlotIndex));
            this.GetSystem<ITurnSystem>().CheckStepComplete();
        }
    }
}