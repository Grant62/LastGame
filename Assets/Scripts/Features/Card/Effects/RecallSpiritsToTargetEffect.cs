using System.Collections.Generic;
using System.Linq;
using Core.Systems;
using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class RecallSpiritsToTargetEffect : Effect
    {
        private readonly int mDamagePerSpirit;

        public RecallSpiritsToTargetEffect(int damagePerSpirit)
        {
            mDamagePerSpirit = damagePerSpirit;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;
            BoardView board = Ctx.BoardAccess.Board;

            List<EnemyView> validEnemies = board.GetActiveEnemies()
                .Where(e => e.IsValidTarget)
                .ToList();

            if (validEnemies.Count == 0)
                return;

            int targetSlot = validEnemies[Ctx.RandomSystem.Range(0, validEnemies.Count, RandomModuleIds.Combat)].SlotIndex;

            List<int> spiritSlots = new(sword.SpiritSwordSlots);
            int spiritCount = spiritSlots.Count;

            foreach (int fromSlot in spiritSlots)
            {
                int pathDmg = Ctx.Config.SpiritPathDamage + sword.CustomPathDamage;
                int step = targetSlot > fromSlot ? 1 : -1;
                for (int i = fromSlot; i != targetSlot + step; i += step)
                {
                    if (board.TryGetEnemyAtSlot(i, out EnemyView enemy) && enemy.IsValidTarget)
                        enemy.TakeDamage(pathDmg);
                }
            }

            sword.IsRecalling = true;
            sword.RecallTargetSlot = targetSlot;
            sword.SpiritSwordSlots.Clear();
            sword.OnSpiritSwordsChanged.Trigger();

            if (spiritCount > 0
                && board.TryGetEnemyAtSlot(targetSlot, out EnemyView targetEnemy)
                && targetEnemy.IsValidTarget)
            {
                for (int i = 0; i < spiritCount; i++)
                    targetEnemy.TakeDamage(mDamagePerSpirit);
            }
        }
    }
}