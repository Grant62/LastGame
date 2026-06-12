using Features.Combat.Event;
using Features.Combat.UI;
using Features.Combat.UI.Board;
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

            BoardPanel board = this.GetUtility<IBoardAccess>().Board;

            int slot = sword.CurSlotIndex.Value;
            int damage = sword.SpinDamage.Value;

            DamageEnemyAtSlot(board, slot, damage);

            if (sword.SpinHitsAdjacent)
            {
                DamageEnemyAtSlot(board, slot - 1, damage);
                DamageEnemyAtSlot(board, slot + 1, damage);
            }
        }

        private static void DamageEnemyAtSlot(BoardPanel board, int slot, int damage)
        {
            if (board.TryGetEnemyAtSlot(slot, out EnemyUI enemy) && enemy.IsValidTarget)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}