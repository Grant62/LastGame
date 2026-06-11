using QFramework;

namespace Core.Systems
{
    public interface IRandomSystem : ISystem
    {
        void Initialize(int? seed = null);
        int Range(int minInclusive, int maxExclusive, int moduleId);
        float Value(int moduleId);
        int RangeForPosition(int minInclusive, int maxExclusive, params int[] positionKeys);
        void SetParentSeed(int parentSeed);
    }
}