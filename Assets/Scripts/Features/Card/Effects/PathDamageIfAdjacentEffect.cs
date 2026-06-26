using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Sword.Model;
using UnityEngine;

namespace Features.Card.Effects
{
    public class PathDamageIfAdjacentEffect : Effect
    {
        private readonly int mDamage;

        public PathDamageIfAdjacentEffect(int damage)
        {
            mDamage = damage;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;
            int oldSlot = sword.CurSlotIndex.Value;
            int targetSlot = Ctx.SlotTargetIndex;

            if (Mathf.Abs(targetSlot - oldSlot) > 1)
                return;

            BoardView board = Ctx.BoardAccess.Board;
            int step = targetSlot > oldSlot ? 1 : -1;
            for (int i = oldSlot; i != targetSlot + step; i += step)
            {
                if (board.TryGetEnemyAtSlot(i, out EnemyView enemy) && enemy.IsValidTarget)
                    enemy.TakeDamage(mDamage);
            }
        }
    }
}