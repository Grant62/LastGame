using QFramework;

namespace Features.Resource.System
{
    public interface IResourceSystem : ISystem
    {
        bool CanSpend(int amount);
        void Spend(int amount);
        void Gain(int amount);
    }
}