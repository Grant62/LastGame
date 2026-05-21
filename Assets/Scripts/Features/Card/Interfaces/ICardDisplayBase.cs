using Features.Card.Data;
using UnityEngine;
using UnityEngine.Rendering;

namespace Features.Card.Interfaces
{
    public interface ICardDisplayBase
    {
        CardData CardData { get; }
        SortingGroup SortingGroup { get; }

        void Setup(CardData data);
        void SetCardData(CardData data);
        void SetTitle(string titleText);
        void SetDescription(string description);
        void SetCost(string costText);
        void SetType(string typeText);
        void SetRarityColor(Color color);
        void SetImage(Sprite sprite);
        void UpdatePositionRotation(Vector3 pos, Quaternion rot, float duration);
        void SetUsable(bool usable);
        void UpdateUsable();
        void ResetWrapper();
    }
}
