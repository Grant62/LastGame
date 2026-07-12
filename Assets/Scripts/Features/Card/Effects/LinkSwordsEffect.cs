using System.Collections.Generic;
using Features.Combat.Interfaces;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Hero.Model;
using Features.Sword.Model;
using UnityEngine;

namespace Features.Card.Effects
{
    public class LinkSwordsEffect : Effect
    {
        private readonly int mBlockPerSword;
        private readonly Effect mPenetrationEffect;

        public LinkSwordsEffect(int blockPerSword = 0, Effect penetrationEffect = null)
        {
            mBlockPerSword = blockPerSword;
            mPenetrationEffect = penetrationEffect;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel swordModel = Ctx.SwordModel;
            IHeroModel heroModel = Ctx.HeroModel;
            int playerSlot = heroModel.CurSlotIndex.Value;

            List<int> swordSlots = swordModel.GetAllSwordSlots();

            int blockPerSword = mBlockPerSword > 0
                ? mBlockPerSword
                : mBlockPerSword < 0
                    ? 0
                    : Ctx.Config.LinkBlockPerSword;
            int totalBlock = blockPerSword * swordSlots.Count;
            if (totalBlock > 0 && caster is IDamageable damageable)
                damageable.GainArmor(totalBlock);

            bool penetrated = IsPenetrated(playerSlot, swordSlots);
            swordModel.LastLinkPenetrated = penetrated;

            if (mPenetrationEffect != null && penetrated)
            {
                mPenetrationEffect.Ctx = Ctx;
                mPenetrationEffect.Execute(targets, caster);
            }
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
            return IsPenetrated(Ctx.SwordModel, Ctx.BoardAccess.Board, playerSlot, swordSlots);
        }

        public static bool IsPenetrated(ISwordModel swordModel, BoardView board, int playerSlot, List<int> swordSlots)
        {
            if (swordModel.LinkAlwaysPenetrate)
                return true;

            if (swordSlots.Count == 0)
                return false;

            HashSet<int> covered = GetCoveredSlots(playerSlot, swordSlots);

            foreach (EnemyView enemy in board.GetActiveEnemies())
            {
                if (enemy.IsValidTarget && !covered.Contains(enemy.SlotIndex))
                    return false;
            }

            return true;
        }
    }
}