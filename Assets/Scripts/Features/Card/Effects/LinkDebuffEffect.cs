using System.Collections.Generic;
using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Hero.Model;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class LinkDebuffEffect : Effect
    {
        private readonly StatusType mStatusType;
        private readonly int mStacks;

        public LinkDebuffEffect(StatusType statusType, int stacks = 2)
        {
            mStatusType = statusType;
            mStacks = stacks;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel swordModel = Ctx.SwordModel;
            IHeroModel heroModel = Ctx.HeroModel;
            int playerSlot = heroModel.CurSlotIndex.Value;

            BoardView board = Ctx.BoardAccess.Board;

            List<int> swordSlots = new();
            if (swordModel.CurSlotIndex.Value >= 0)
                swordSlots.Add(swordModel.CurSlotIndex.Value);
            swordSlots.AddRange(swordModel.SpiritSwordSlots);

            HashSet<int> covered = LinkSwordsEffect.GetCoveredSlots(playerSlot, swordSlots);

            foreach (EnemyView enemy in board.GetActiveEnemies())
            {
                if (enemy.IsValidTarget && covered.Contains(enemy.SlotIndex))
                    StatusHelper.ApplyStatus(enemy.Statuses, mStatusType, mStacks);
            }
        }
    }
}