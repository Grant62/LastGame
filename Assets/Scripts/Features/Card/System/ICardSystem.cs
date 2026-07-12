using System.Collections.Generic;
using Features.Card.Data;
using QFramework;

namespace Features.Card.System
{
    public interface ICardSystem : ISystem
    {
        void InitLibrary(List<CardData> cards);
        void StartBattleDraw();
        void DrawCards(int count);
        void RemoveFromHand(CardData card);
        void DiscardFromHand(CardData card);
        void AddToDiscard(CardData card);
        void AddToHand(CardData card);
        void AddToConsume(CardData card);
        void ShuffleDrawPile();
        void AddToLibrary(CardData card);
        void RemoveFromLibrary(CardData card);
        void ReturnToHand(CardData card);
    }
}