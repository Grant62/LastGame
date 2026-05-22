using Features.Card.Data;
using Features.Combat.Targeting;

namespace Features.Card.Event
{
    public struct CardPlayedEvent
    {
        public CardData CardData { get; }
        public ITargetable ManualTarget { get; }

        public CardPlayedEvent(CardData cardData, ITargetable manualTarget = null)
        {
            CardData = cardData;
            ManualTarget = manualTarget;
        }
    }
}