using System;

namespace Core.Systems
{
    public interface IRandomSystem
    {
        void Initialize(int? seed = null);
        Random GetRandomGenerator(int moduleId);
        int Range(int minInclusive, int maxExclusive, int moduleId);
        float Value(int moduleId);
        int RangeForPosition(int minInclusive, int maxExclusive, params int[] positionKeys);
        void SetParentSeed(int parentSeed);
    }
}
