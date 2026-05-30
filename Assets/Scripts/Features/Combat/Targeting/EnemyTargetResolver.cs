using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Features.Combat.Targeting
{
    public class EnemyTargetResolver : ITargetResolver
    {
        public ITargetable[] Resolve(TargetType type, ITargetable caster)
        {
            if (type is TargetType.RandomEnemy or TargetType.AllEnemies)
            {
                IDamageable[] enemies = Object.FindObjectsByType<MonoBehaviour>(
                    FindObjectsInactive.Exclude, FindObjectsSortMode.None
                ).OfType<IDamageable>().ToArray();

                if (enemies.Length == 0)
                    return Array.Empty<ITargetable>();

                if (type == TargetType.RandomEnemy)
                    return new ITargetable[] { enemies[Random.Range(0, enemies.Length)] };

                return enemies.Cast<ITargetable>().ToArray();
            }

            if (type == TargetType.Self && caster != null)
                return new[] { caster };

            return Array.Empty<ITargetable>();
        }
    }
}