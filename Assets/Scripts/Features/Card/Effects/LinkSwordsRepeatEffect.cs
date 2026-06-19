using System.Collections.Generic;
using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Hero.Model;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class LinkSwordsRepeatEffect : Effect
    {
        private readonly int mBlockPerSword;

        public LinkSwordsRepeatEffect(int blockPerSword = 0)
        {
            mBlockPerSword = blockPerSword;
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
            int totalBlock = blockPerSword * swordSlots.Count;
            if (totalBlock > 0 && caster is IDamageable damageable)
                damageable.GainArmor(totalBlock);

            BoardView board = Ctx.BoardAccess.Board;
            bool penetrated = IsPenetrated(playerSlot, swordSlots, board);
            swordModel.LastLinkPenetrated = penetrated;
            if (penetrated)
            {
                // Second link: grant armor again
                totalBlock = blockPerSword * swordSlots.Count;
                if (totalBlock > 0 && caster is IDamageable d)
                    d.GainArmor(totalBlock);
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