using System.Collections.Generic;
using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Hero.Model;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class RecallSwordsEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel swordModel = Ctx.SwordModel;
            IHeroModel heroModel = Ctx.HeroModel;
            BoardView board = Ctx.BoardAccess.Board;
            int playerSlot = heroModel.CurSlotIndex.Value;

            List<int> swordSlots = new();
            if (swordModel.CurSlotIndex.Value >= 0)
                swordSlots.Add(swordModel.CurSlotIndex.Value);
            swordSlots.AddRange(swordModel.SpiritSwordSlots);

            HashSet<int> damaged = new();
            foreach (int fromSlot in swordSlots)
            {
                bool isSpirit = swordModel.SpiritSwordSlots.Contains(fromSlot);
                int pathDmg = isSpirit ? Ctx.Config.SpiritPathDamage : Ctx.Config.SwordPathDamage;
                int step = playerSlot > fromSlot ? 1 : -1;
                for (int i = fromSlot; i != playerSlot + step; i += step)
                {
                    if (damaged.Add(i) && board.TryGetEnemyAtSlot(i, out EnemyView enemy) && enemy.IsValidTarget)
                        enemy.TakeDamage(pathDmg);
                }
            }

            swordModel.CurSlotIndex.Value = playerSlot;

            if (swordModel.SpiritSwordSlots.Count > 0)
            {
                swordModel.SpiritSwordSlots.Clear();
                swordModel.OnSpiritSwordsChanged.Trigger();
            }
        }
    }
}