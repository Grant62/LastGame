using System;
using System.Collections.Generic;
using QFramework;

namespace Core.Systems
{
    public class RandomSystem : AbstractSystem, IRandomSystem
    {
        private int mParentSeed;
        private readonly Dictionary<int, Xoshiro128> mGenerators = new();

        protected override void OnInit()
        {
            mParentSeed = GenerateParentSeed();
        }

        public void Initialize(int? seed = null)
        {
            mParentSeed = seed ?? GenerateParentSeed();
            mGenerators.Clear();
        }

        public int Range(int minInclusive, int maxExclusive, int moduleId)
        {
            return GetGenerator(moduleId).Next(minInclusive, maxExclusive);
        }

        public float Value(int moduleId)
        {
            return GetGenerator(moduleId).NextFloat();
        }

        public int RangeForPosition(int minInclusive, int maxExclusive, params int[] positionKeys)
        {
            int skip = 0;
            unchecked
            {
                foreach (int key in positionKeys)
                    skip = skip * 31 + key;
            }

            Xoshiro128 gen = GetGenerator(RandomModuleIds.Combat);
            for (int i = 0; i < skip % 37; i++)
                gen.Next(0, 1);

            return gen.Next(minInclusive, maxExclusive);
        }

        public void SetParentSeed(int parentSeed)
        {
            mParentSeed = parentSeed;
            mGenerators.Clear();
        }

        private Xoshiro128 GetGenerator(int moduleId)
        {
            if (!mGenerators.TryGetValue(moduleId, out Xoshiro128 generator))
            {
                int childSeed = mParentSeed ^ moduleId * 397;
                generator = new Xoshiro128(childSeed);
                mGenerators[moduleId] = generator;
            }

            return generator;
        }

        private static int GenerateParentSeed()
        {
            unchecked
            {
                long ticks = DateTime.Now.Ticks;
                int hash = Guid.NewGuid().GetHashCode();
                return (int)(ticks & 0xFFFFFFFF) ^ hash;
            }
        }

        private sealed class Xoshiro128
        {
            private uint mS0, mS1, mS2, mS3;

            public Xoshiro128(int seed)
            {
                mS0 = SplitMix32(ref seed);
                mS1 = SplitMix32(ref seed);
                mS2 = SplitMix32(ref seed);
                mS3 = SplitMix32(ref seed);
            }

            public int Next(int minInclusive, int maxExclusive)
            {
                uint range = (uint)(maxExclusive - minInclusive);
                return minInclusive + (int)(NextUInt() % range);
            }

            public float NextFloat()
            {
                return (NextUInt() >> 8) * (1f / (1u << 24));
            }

            private uint NextUInt()
            {
                uint result = Rotl(mS1 * 5, 7) * 9;
                uint t = mS1 << 9;
                mS2 ^= mS0;
                mS3 ^= mS1;
                mS1 ^= mS2;
                mS0 ^= mS3;
                mS2 ^= t;
                mS3 = Rotl(mS3, 11);
                return result;
            }

            private static uint Rotl(uint x, int k)
            {
                return x << k | x >> 32 - k;
            }

            private static uint SplitMix32(ref int state)
            {
                state = state * -299072291 + 1; // 1664525 * 2^32 + 1013904223 approx
                uint z = (uint)state;
                z = (z ^ z >> 15) * 0x85EBCA77;
                z = (z ^ z >> 13) * 0xC2B2AE35;
                return z ^ z >> 16;
            }
        }
    }
}