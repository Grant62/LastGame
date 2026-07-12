using System.Collections.Generic;
using System.Linq;
using Core.Systems;
using Features.Combat.Interfaces;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class SpinDamageRandomEnemyEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;
            int damage = sword.SpinDamage.Value;
            if (damage <= 0)
                return;

            BoardView board = Ctx.BoardAccess.Board;
            List<EnemyView> validEnemies = board.GetActiveEnemies()
                .Where(e => e.IsValidTarget)
                .ToList();

            if (validEnemies.Count == 0)
                return;

            int index = Ctx.RandomSystem.Range(0, validEnemies.Count, RandomModuleIds.Combat);
            validEnemies[index].TakeDamage(damage);
        }
    }
}