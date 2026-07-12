using System.Collections.Generic;
using System.Linq;
using Features.Combat.Interfaces;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Hero.Model;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class SpawnSpiritsByArmorEffect : Effect
    {
        private readonly int mArmorPerSpirit;

        public SpawnSpiritsByArmorEffect(int armorPerSpirit = 6)
        {
            mArmorPerSpirit = armorPerSpirit;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            IHeroModel hero = Ctx.HeroModel;
            ISwordModel sword = Ctx.SwordModel;
            BoardView board = Ctx.BoardAccess.Board;

            HashSet<int> occupied = new(sword.SpiritSwordSlots);

            int armor = hero.Armor.Value;
            List<int> enemySlots = new();
            List<int> emptySlots = new();

            for (int slot = 0; slot < 9; slot++)
            {
                if (occupied.Contains(slot))
                    continue;

                if (board.TryGetEnemyAtSlot(slot, out EnemyView enemy) && enemy.IsValidTarget)
                    enemySlots.Add(slot);
                else
                    emptySlots.Add(slot);
            }

            foreach (int slot in enemySlots.Concat(emptySlots))
            {
                if (armor < mArmorPerSpirit)
                    break;

                armor -= mArmorPerSpirit;
                sword.SpiritSwordSlots.Add(slot);
            }

            hero.Armor.Value = armor;
            sword.OnSpiritSwordsChanged.Trigger();
        }
    }
}