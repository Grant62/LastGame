using System.Collections.Generic;
using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Hero.Model;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class RecallSpinDamageEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;
            int damage = sword.SpinDamage.Value;
            if (damage <= 0)
                return;

            IHeroModel hero = Ctx.HeroModel;
            BoardView board = Ctx.BoardAccess.Board;
            int playerSlot = hero.CurSlotIndex.Value;

            List<int> swordSlots = new();
            if (sword.CurSlotIndex.Value >= 0)
                swordSlots.Add(sword.CurSlotIndex.Value);
            swordSlots.AddRange(sword.SpiritSwordSlots);

            foreach (int fromSlot in swordSlots)
            {
                int step = playerSlot > fromSlot ? 1 : -1;
                for (int i = fromSlot; i != playerSlot + step; i += step)
                {
                    if (board.TryGetEnemyAtSlot(i, out EnemyView enemy) && enemy.IsValidTarget)
                        enemy.TakeDamage(damage);
                }
            }
        }
    }
}