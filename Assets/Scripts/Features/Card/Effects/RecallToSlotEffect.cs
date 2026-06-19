using System.Collections.Generic;
using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class RecallToSlotEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel swordModel = Ctx.SwordModel;
            BoardView board = Ctx.BoardAccess.Board;
            int targetSlot = Ctx.SlotTargetIndex;

            List<int> swordSlots = new();
            if (swordModel.CurSlotIndex.Value >= 0)
                swordSlots.Add(swordModel.CurSlotIndex.Value);
            swordSlots.AddRange(swordModel.SpiritSwordSlots);

            foreach (int fromSlot in swordSlots)
            {
                bool isSpirit = swordModel.SpiritSwordSlots.Contains(fromSlot);
                int pathDmg = isSpirit ? Ctx.Config.SpiritPathDamage : Ctx.Config.SwordPathDamage;
                if (swordModel.CustomPathDamage > 0)
                    pathDmg = swordModel.CustomPathDamage;
                int step = targetSlot > fromSlot ? 1 : -1;
                for (int i = fromSlot; i != targetSlot + step; i += step)
                {
                    if (board.TryGetEnemyAtSlot(i, out EnemyView enemy) && enemy.IsValidTarget)
                        enemy.TakeDamage(pathDmg);
                }
            }

            swordModel.CurSlotIndex.Value = targetSlot;

            if (swordModel.SpiritSwordSlots.Count > 0)
            {
                swordModel.IsRecalling = true;
                swordModel.SpiritSwordSlots.Clear();
                swordModel.OnSpiritSwordsChanged.Trigger();
            }
        }
    }
}