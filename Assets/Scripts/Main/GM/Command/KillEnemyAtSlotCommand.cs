using Features.Combat.Utility;
using Features.Combat.View.Board;
using Features.Enemy.View;
using QFramework;

namespace Main.GM.Command
{
    public class KillEnemyAtSlotCommand : AbstractCommand
    {
        private readonly int mSlotIndex;

        public KillEnemyAtSlotCommand(int slotIndex)
        {
            mSlotIndex = slotIndex;
        }

        protected override void OnExecute()
        {
            BoardView board = this.GetUtility<IBoardAccess>().Board;
            EnemyView enemy = board.GetEnemyAtSlot(mSlotIndex);
            if (enemy != null && enemy.IsValidTarget)
                enemy.ApplyDamage(999999);
        }
    }
}