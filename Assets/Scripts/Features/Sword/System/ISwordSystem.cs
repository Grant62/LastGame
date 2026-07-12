using QFramework;

namespace Features.Sword.System
{
    public interface ISwordSystem : ISystem
    {
        void ReduceSpinCardCosts();
        void RestoreCardCosts();
    }
}