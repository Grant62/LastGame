using Features.Combat.Targeting;
using Features.Combat.UI.Board;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class SpinDamageImmediateEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;
            int damage = sword.SpinDamage.Value;
            if (damage <= 0) return;

            int slot = sword.CurSlotIndex.Value;
            BoardPanel board = Ctx.BoardAccess.Board;

            DamageEnemyAtSlot(board, slot, damage);
            DamageEnemyAtSlot(board, slot - 1, damage);
            DamageEnemyAtSlot(board, slot + 1, damage);
        }

        private static void DamageEnemyAtSlot(BoardPanel board, int slot, int damage)
        {
            if (board.TryGetEnemyAtSlot(slot, out EnemyUI enemy) && enemy.IsValidTarget)
                enemy.TakeDamage(damage);
        }
    }
}