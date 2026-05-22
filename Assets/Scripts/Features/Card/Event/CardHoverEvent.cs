using Features.Card.Data;
using UnityEngine;

namespace Features.Card.Event
{
    public struct CardHoverEvent
    {
        public CardData CardData { get; }
        public Vector3 Position { get; }

        public CardHoverEvent(CardData cardData, Vector3 position)
        {
            CardData = cardData;
            Position = position;
        }
    }

    public struct CardHoverEndEvent { }
}