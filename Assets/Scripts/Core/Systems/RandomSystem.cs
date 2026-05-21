using System;
using System.Collections.Generic;
using QFramework;

namespace Core.Systems
{
    public class RandomSystem : AbstractSystem, IRandomSystem
    {
        private int mParentSeed;
        private Dictionary<int, Random> mRandomGenerators;

        protected override void OnInit()
        {
            mParentSeed = GenerateParentSeed();
            mRandomGenerators = new Dictionary<int, Random>();
        }

        public void Initialize(int? seed = null)
        {
            mParentSeed = seed ?? GenerateParentSeed();
            mRandomGenerators.Clear();
        }

        public Random GetRandomGenerator(int moduleId)
        {
            if (!mRandomGenerators.TryGetValue(moduleId, out Random random))
            {
                int seed = GetSeedForModule(moduleId);
                random = new Random(seed);
                mRandomGenerators[moduleId] = random;
            }

            return random;
        }

        public int Range(int minInclusive, int maxExclusive, int moduleId)
        {
            Random random = GetRandomGenerator(moduleId);
            return random.Next(minInclusive, maxExclusive);
        }

        public float Value(int moduleId)
        {
            Random random = GetRandomGenerator(moduleId);
            return (float)random.NextDouble();
        }

        public int RangeForPosition(int minInclusive, int maxExclusive, params int[] positionKeys)
        {
            int seed = GeneratePositionSeed(positionKeys);
            Random random = new(seed);
            return random.Next(minInclusive, maxExclusive);
        }

        public void SetParentSeed(int parentSeed)
        {
            mParentSeed = parentSeed;
            mRandomGenerators.Clear();
        }

        private int GeneratePositionSeed(params int[] positionKeys)
        {
            unchecked
            {
                int seed = mParentSeed;

                foreach (int key in positionKeys)
                {
                    seed = seed * 31 + key;
                }

                return seed;
            }
        }

        private int GetSeedForModule(int moduleId)
        {
            unchecked
            {
                return mParentSeed ^ moduleId * 397;
            }
        }

        private int GenerateParentSeed()
        {
            unchecked
            {
                long ticks = DateTime.Now.Ticks;
                int hash = Guid.NewGuid().GetHashCode();
                return (int)(ticks & 0xFFFFFFFF) ^ hash;
            }
        }
    }
}
