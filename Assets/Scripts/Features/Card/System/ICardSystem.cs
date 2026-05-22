using System.Collections.Generic;
using Features.Card.Data;
using QFramework;

namespace Features.Card.System
{
    public interface ICardSystem : ISystem
    {
        void DrawCards(int count);
        void RemoveFromHand(CardData card);
        void DiscardFromHand(CardData card);
        void AddToDiscard(CardData card);
        void ShuffleDrawPile();
        void InitDrawPile(List<CardData> cards);
    }
}