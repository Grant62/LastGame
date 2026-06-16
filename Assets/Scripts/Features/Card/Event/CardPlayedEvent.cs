using Features.Card.Data;
using Features.Combat.Targeting;

namespace Features.Card.Event
{
    public struct CardPlayedEvent
    {
        public CardData CardData { get; }
        public ITargetable ManualTarget { get; }
        public int EnergySpent { get; }
        public int SlotTargetIndex { get; }

        public CardPlayedEvent(CardData cardData, ITargetable manualTarget = null, int energySpent = 0, int slotTargetIndex = -1)
        {
            CardData = cardData;
            ManualTarget = manualTarget;
            EnergySpent = energySpent;
            SlotTargetIndex = slotTargetIndex;
        }
    }
}