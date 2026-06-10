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
        void ShuffleDrawPile();
    }
}