using QFramework;
using UnityEngine;

namespace Features.Card.Utility
{
    public interface ICardSpriteCache : IUtility
    {
        Sprite GetSprite(string iconAddress);
    }
}