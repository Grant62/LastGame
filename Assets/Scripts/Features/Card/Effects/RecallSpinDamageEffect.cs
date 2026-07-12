using System.Collections.Generic;
using Features.Combat.Interfaces;
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

            List<int> swordSlots = sword.GetAllSwordSlots();

            foreach (int fromSlot in swordSlots)
            {
                board.ForEachSlotOnPath(fromSlot, playerSlot, i =>
                {
                    if (board.TryGetEnemyAtSlot(i, out EnemyView enemy) && enemy.IsValidTarget)
                        enemy.TakeDamage(damage);
                });
            }
        }
    }
}