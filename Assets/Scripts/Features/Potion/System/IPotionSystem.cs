using Features.Combat.Interfaces;
using QFramework;

namespace Features.Potion.System
{
    public interface IPotionSystem : ISystem
    {
        void UsePotion(int slotIndex, ITargetable target = null);
    }
}