using System.Collections.Generic;
using Features.Combat.Interfaces;
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

            List<int> swordSlots = swordModel.GetAllSwordSlots();

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

            bool penetrated = LinkSwordsEffect.IsPenetrated(Ctx.SwordModel, board, playerSlot, swordSlots);
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
    }
}