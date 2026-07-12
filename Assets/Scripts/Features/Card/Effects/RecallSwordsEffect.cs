using System.Collections.Generic;
using Features.Combat.Interfaces;
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

            List<int> swordSlots = swordModel.GetAllSwordSlots();

            HashSet<int> attachSlots = new();

            foreach (int fromSlot in swordSlots)
            {
                bool isSpirit = swordModel.SpiritSwordSlots.Contains(fromSlot);
                int pathDmg = (isSpirit ? Ctx.Config.SpiritPathDamage : Ctx.Config.SwordPathDamage) + swordModel.CustomPathDamage;
                board.ForEachSlotOnPath(fromSlot, playerSlot, i =>
                {
                    if (board.TryGetEnemyAtSlot(i, out EnemyView enemy) && enemy.IsValidTarget)
                    {
                        enemy.TakeDamage(pathDmg);
                        if (swordModel.IsSpiritAttached.Value)
                            attachSlots.Add(i);
                    }
                });
            }

            swordModel.CurSlotIndex.Value = playerSlot;

            if (swordModel.SpiritSwordSlots.Count > 0)
            {
                swordModel.IsRecalling = true;
                swordModel.RecallTargetSlot = playerSlot;
                swordModel.SpiritSwordSlots.Clear();
                swordModel.OnSpiritSwordsChanged.Trigger();
            }

            foreach (int slot in attachSlots)
            {
                if (!swordModel.SpiritSwordSlots.Contains(slot))
                    swordModel.SpiritSwordSlots.Add(slot);
            }

            if (attachSlots.Count > 0)
                swordModel.OnSpiritSwordsChanged.Trigger();
        }
    }
}