using Features.Card.Data;

namespace Features.Card.Event
{
    public struct CardSelectedEvent
    {
        public CardData CardData { get; }

        public CardSelectedEvent(CardData cardData)
        {
            CardData = cardData;
        }
    }
}