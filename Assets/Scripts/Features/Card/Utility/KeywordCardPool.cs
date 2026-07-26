using Features.Card.View;
using QFramework;
using UnityEngine;

namespace Features.Card.Utility
{
    public class KeywordCardPool : IKeywordCardPool
    {
        private readonly SimpleObjectPool<KeywordCard> mPool;
        private readonly Transform mPoolRoot;
        private readonly KeywordCard mPrefab;

        public KeywordCardPool(KeywordCard prefab, Transform parent)
        {
            mPrefab = prefab;
            GameObject go = new("[Pool] KeywordCard");
            go.transform.SetParent(parent, false);
            mPoolRoot = go.transform;

            mPool = new SimpleObjectPool<KeywordCard>(
                () =>
                {
                    KeywordCard card = Object.Instantiate(prefab, mPoolRoot);
                    card.gameObject.SetActive(false);
                    return card;
                },
                null,
                8
            );

            go.SetActive(false);
        }

        public KeywordCard Get(Transform parent)
        {
            KeywordCard card = mPool.Allocate();
            if (card == null || !card)
            {
                card = Object.Instantiate(mPrefab, mPoolRoot);
                card.gameObject.SetActive(false);
            }

            card.transform.SetParent(parent, false);
            card.gameObject.SetActive(true);
            return card;
        }

        public void Return(KeywordCard card)
        {
            card.gameObject.SetActive(false);
            card.transform.SetParent(mPoolRoot, false);
            mPool.Recycle(card);
        }

        public void Dispose()
        {
            Object.Destroy(mPoolRoot.gameObject);
        }
    }
}
