using Features.Card.Data;
using Features.Card.View;
using QFramework;
using UnityEngine;

namespace Features.Card.Interfaces
{
    public interface ICardViewPool : IUtility
    {
        CardView Get(CardData data, Transform parent);
        void Return(CardView view);
    }
}