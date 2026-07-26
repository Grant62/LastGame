using Features.Card.View;
using QFramework;
using UnityEngine;

namespace Features.Card.Utility
{
    public interface IKeywordCardPool : IUtility
    {
        KeywordCard Get(Transform parent);
        void Return(KeywordCard card);
        void Dispose();
    }
}
