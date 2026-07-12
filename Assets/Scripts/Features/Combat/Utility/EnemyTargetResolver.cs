using System;
using System.Collections.Generic;
using Core.Systems;
using Features.Combat.Define;
using Features.Combat.Interfaces;

namespace Features.Combat.Utility
{
    public class EnemyTargetResolver : ITargetResolver
    {
        private readonly Func<IReadOnlyList<IDamageable>> mEnemyProvider;
        private readonly IRandomSystem mRandomSystem;
        private readonly ITargetable[] mSingleTarget = new ITargetable[1];

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
                    mSingleTarget[0] = enemies[mRandomSystem.Range(0, enemies.Count, RandomModuleIds.Combat)];
                    return mSingleTarget;
                }
                case TargetType.AllEnemies:
                    return ConvertAll(mEnemyProvider());
                case TargetType.Self:
                    return caster != null ? new[] { caster } : Array.Empty<ITargetable>();
                default:
                    return Array.Empty<ITargetable>();
            }
        }

        private static ITargetable[] ConvertAll(IReadOnlyList<IDamageable> list)
        {
            ITargetable[] result = new ITargetable[list.Count];
            for (int i = 0; i < list.Count; i++)
                result[i] = list[i];
            return result;
        }
    }
}