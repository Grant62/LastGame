using System.Collections.Generic;
using Features.Combat.Interfaces;
using Features.Combat.View.Board;
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

            List<int> swordSlots = swordModel.GetAllSwordSlots();

            int blockPerSword = mBlockPerSword > 0
                ? mBlockPerSword
                : mBlockPerSword < 0
                    ? 0
                    : Ctx.Config.LinkBlockPerSword;
            int totalBlock = blockPerSword * swordSlots.Count;
            if (totalBlock > 0 && caster is IDamageable damageable)
                damageable.GainArmor(totalBlock);

            BoardView board = Ctx.BoardAccess.Board;
            bool penetrated = LinkSwordsEffect.IsPenetrated(Ctx.SwordModel, board, playerSlot, swordSlots);
            swordModel.LastLinkPenetrated = penetrated;
            if (penetrated)
            {
                // Second link: grant armor again
                totalBlock = blockPerSword * swordSlots.Count;
                if (totalBlock > 0 && caster is IDamageable d)
                    d.GainArmor(totalBlock);
            }
        }
    }
}