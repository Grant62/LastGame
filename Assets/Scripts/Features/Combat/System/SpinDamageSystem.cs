using Features.Combat.Event;
using Features.Combat.Utility;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Sword.Model;
using QFramework;

namespace Features.Combat.System
{
    public class SpinDamageSystem : AbstractSystem
    {
        protected override void OnInit()
        {
            this.RegisterEvent<PlayerTurnEndEvent>(OnPlayerTurnEnd);
        }

        private void OnPlayerTurnEnd(PlayerTurnEndEvent e)
        {
            ISwordModel sword = this.GetModel<ISwordModel>();
            if (!sword.IsSpinning.Value || sword.SpinDamage.Value <= 0)
                return;

            BoardView board = this.GetUtility<IBoardAccess>().Board;

            int slot = sword.CurSlotIndex.Value;
            int damage = sword.SpinDamage.Value;

            DamageEnemyAtSlot(board, slot, damage);

            if (sword.SpinHitsAdjacent)
            {
                DamageEnemyAtSlot(board, slot - 1, damage);
                DamageEnemyAtSlot(board, slot + 1, damage);
            }
        }

        private static void DamageEnemyAtSlot(BoardView board, int slot, int damage)
        {
            if (board.TryGetEnemyAtSlot(slot, out EnemyView enemy) && enemy.IsValidTarget)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}