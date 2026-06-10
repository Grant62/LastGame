using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Features.Combat.Targeting
{
    public class EnemyTargetResolver : ITargetResolver
    {
        private readonly Func<IReadOnlyList<IDamageable>> mEnemyProvider;

        public EnemyTargetResolver(Func<IReadOnlyList<IDamageable>> enemyProvider)
        {
            mEnemyProvider = enemyProvider;
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
                    return new ITargetable[] { enemies[Random.Range(0, enemies.Count)] };
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