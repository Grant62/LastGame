using Features.Card.Data;
using Features.Card.View;
using QFramework;
using UnityEngine;

namespace Features.Card.Utility
{
    public interface ICardViewPool : IUtility
    {
        CardView Get(CardData data, Transform parent, bool enableEffects = true);
        void Return(CardView view);
        void Dispose();
    }
}