using System.Collections.Generic;
using Features.Combat.Interfaces;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Hero.Model;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class PerLinkedEnemyEffect : Effect
    {
        private readonly Effect mPerEnemyEffect;

        public PerLinkedEnemyEffect(Effect perEnemyEffect)
        {
            mPerEnemyEffect = perEnemyEffect;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;
            IHeroModel hero = Ctx.HeroModel;
            BoardView board = Ctx.BoardAccess.Board;

            List<int> swordSlots = sword.GetAllSwordSlots();

            HashSet<int> covered = LinkSwordsEffect.GetCoveredSlots(hero.CurSlotIndex.Value, swordSlots);

            int enemyCount = 0;
            foreach (EnemyView enemy in board.GetActiveEnemies())
            {
                if (enemy.IsValidTarget && covered.Contains(enemy.SlotIndex))
                    enemyCount++;
            }

            mPerEnemyEffect.Ctx = Ctx;
            for (int i = 0; i < enemyCount; i++)
                mPerEnemyEffect.Execute(targets, caster);
        }
    }
}