using System;
using System.Collections.Generic;
using System.Linq;
using Core.Systems;

namespace Features.Combat.Targeting
{
    public class EnemyTargetResolver : ITargetResolver
    {
        private readonly Func<IReadOnlyList<IDamageable>> mEnemyProvider;
        private readonly IRandomSystem mRandomSystem;

        public EnemyTargetResolver(Func<IReadOnlyList<IDamageable>> enemyProvider, IRandomSystem randomSystem)
        {
            mEnemyProvider = enemyProvider;
            mRandomSystem = randomSystem;
        }

        public ITargetable[] Resolve(TargetType type, ITargetable caster)
        {
            switch (type)
            {
                case TargetType.RandomEnemy:
                {
                    IReadOnlyList<IDamageable> enemies = mEnemyProvider();
                    if (enemies.Count == 0)
                        return Array.Empty<ITargetable>();
                    return new ITargetable[] { enemies[mRandomSystem.Range(0, enemies.Count, RandomModuleIds.Combat)] };
                }
                case TargetType.AllEnemies:
                    return mEnemyProvider().Cast<ITargetable>().ToArray();
                case TargetType.Self:
                    return caster != null ? new[] { caster } : Array.Empty<ITargetable>();
                default:
                    return Array.Empty<ITargetable>();
            }
        }
    }
}