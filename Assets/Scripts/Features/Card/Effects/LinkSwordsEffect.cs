using System.Collections.Generic;
using Features.Combat.Targeting;
using Features.Combat.UI.Board;
using Features.Hero.Model;
using Features.Sword.Model;
using UnityEngine;

namespace Features.Card.Effects
{
    public class LinkSwordsEffect : Effect
    {
        private readonly int mBlockPerSword;
        private readonly Effect mPenetrationEffect;

        public LinkSwordsEffect(int blockPerSword = 8, Effect penetrationEffect = null)
        {
            mBlockPerSword = blockPerSword;
            mPenetrationEffect = penetrationEffect;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel swordModel = Ctx.SwordModel;
            IHeroModel heroModel = Ctx.HeroModel;
            int playerSlot = heroModel.CurSlotIndex.Value;

            List<int> swordSlots = new();
            if (swordModel.CurSlotIndex.Value >= 0)
                swordSlots.Add(swordModel.CurSlotIndex.Value);
            swordSlots.AddRange(swordModel.SpiritSwordSlots);

            int totalBlock = mBlockPerSword * swordSlots.Count;
            if (totalBlock > 0 && caster is IDamageable damageable)
                damageable.GainArmor(totalBlock);

            if (mPenetrationEffect != null && IsPenetrated(playerSlot, swordSlots))
                mPenetrationEffect.Execute(targets, caster);
        }

        public static HashSet<int> GetCoveredSlots(int playerSlot, List<int> swordSlots)
        {
            HashSet<int> covered = new();
            foreach (int slot in swordSlots)
            {
                int from = Mathf.Min(playerSlot, slot);
                int to = Mathf.Max(playerSlot, slot);
                for (int i = from; i <= to; i++)
                    covered.Add(i);
            }

            return covered;
        }

        private bool IsPenetrated(int playerSlot, List<int> swordSlots)
        {
            BoardPanel board = Ctx.BoardAccess.Board;
            if (swordSlots.Count == 0)
                return false;

            HashSet<int> covered = GetCoveredSlots(playerSlot, swordSlots);

            foreach (EnemyUI enemy in board.GetActiveEnemies())
            {
                if (enemy.IsValidTarget && !covered.Contains(enemy.SlotIndex))
                    return false;
            }

            return true;
        }
    }
}