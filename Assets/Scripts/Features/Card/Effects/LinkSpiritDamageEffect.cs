using System.Collections.Generic;
using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Hero.Model;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class LinkSpiritDamageEffect : Effect
    {
        private readonly int mDamagePerSpirit;

        public LinkSpiritDamageEffect(int damagePerSpirit = 6)
        {
            mDamagePerSpirit = damagePerSpirit;
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

            HashSet<int> covered = LinkSwordsEffect.GetCoveredSlots(playerSlot, swordSlots);
            BoardView board = Ctx.BoardAccess.Board;

            foreach (int slot in swordModel.SpiritSwordSlots)
            {
                if (!covered.Contains(slot))
                    continue;

                if (board.TryGetEnemyAtSlot(slot, out EnemyView enemy) && enemy.IsValidTarget)
                    enemy.TakeDamage(mDamagePerSpirit);
            }
        }
    }
}