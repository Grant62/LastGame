using Features.Card.Data;
using Features.Card.UI;
using QFramework;
using UnityEngine;

namespace Features.Card.Interfaces
{
    public interface ICardUIPool : IUtility
    {
        CardUI Get(CardData data, Transform parent);
        void Return(CardUI view);
    }
}