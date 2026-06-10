using Features.Card.Data;
using QFramework;

namespace Features.Sword.System
{
    public interface ISlotTargetSystem : ISystem
    {
        bool Validate(CardData cardData, int targetSlotIndex);
    }
}