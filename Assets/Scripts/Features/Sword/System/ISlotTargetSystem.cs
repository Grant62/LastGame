using QFramework;

namespace Features.Sword.System
{
    public interface ISlotTargetSystem : ISystem
    {
        bool Validate(int cardId, string desc, int targetSlotIndex);
    }
}