using Features.Card.Data;
using QFramework;
using UnityEngine;

namespace Features.Card.Interfaces
{
    public interface ICardHoverDisplay : IUtility
    {
        void Show(CardData data, Vector3 position);
        void Hide();
    }
}