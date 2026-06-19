using System.Collections.Generic;
using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Hero.Model;
using Features.Sword.Model;
using UnityEngine;

namespace Features.Card.Effects
{
    public class LinkSacrificeBlockEffect : Effect
    {
        private readonly int mBlockPerSword;
        private readonly float mRecoveryRatio;

        public LinkSacrificeBlockEffect(int blockPerSword = 0, float recoveryRatio = 0f)
        {
            mBlockPerSword = blockPerSword;
            mRecoveryRatio = recoveryRatio;
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

            int blockPerSword = mBlockPerSword > 0
                ? mBlockPerSword
                : mBlockPerSword < 0
                    ? 0
                    : Ctx.Config.LinkBlockPerSword;
            int linkArmor = blockPerSword * swordSlots.Count;
            if (linkArmor > 0 && caster is IDamageable linkTarget)
                linkTarget.GainArmor(linkArmor);

            int totalArmor = heroModel.Armor.Value;
            if (totalArmor <= 0)
                return;

            heroModel.Armor.Value = 0;

            BoardView board = Ctx.BoardAccess.Board;
            foreach (EnemyView enemy in board.GetActiveEnemies())
            {
                if (enemy.IsValidTarget)
                    enemy.TakeDamage(totalArmor);
            }

            bool penetrated = IsPenetrated(playerSlot, swordSlots, board);
            swordModel.LastLinkPenetrated = penetrated;
            if (!penetrated || mRecoveryRatio <= 0f)
                return;

            if (caster is IDamageable restoreTarget)
            {
                int restore = Mathf.RoundToInt(totalArmor * mRecoveryRatio);
                if (restore > 0)
                    restoreTarget.GainArmor(restore);
            }
        }

        private bool IsPenetrated(int playerSlot, List<int> swordSlots, BoardView board)
        {
            if (Ctx.SwordModel.LinkAlwaysPenetrate)
                return true;

            if (swordSlots.Count == 0)
                return false;

            HashSet<int> covered = LinkSwordsEffect.GetCoveredSlots(playerSlot, swordSlots);

            foreach (EnemyView enemy in board.GetActiveEnemies())
            {
                if (enemy.IsValidTarget && !covered.Contains(enemy.SlotIndex))
                    return false;
            }

            return true;
        }
    }
}