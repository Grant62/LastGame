using Features.Card.Data;
using Features.Card.Interfaces;
using Features.Card.View;
using QFramework;
using UnityEngine;

namespace Features.Card.Pool
{
    public class CardViewPool : ICardViewPool
    {
        private readonly CardView mPrefab;
        private readonly SimpleObjectPool<CardView> mPool;

        public CardViewPool(CardView prefab)
        {
            mPrefab = prefab;
            mPool = new SimpleObjectPool<CardView>(
                () => Object.Instantiate(mPrefab),
                view => view.gameObject.SetActive(false)
            );
        }

        public CardView Get(CardData data, Transform parent)
        {
            CardView view = mPool.Allocate();
            view.transform.SetParent(parent);
            view.transform.localScale = Vector3.one;
            view.gameObject.SetActive(true);
            view.Setup(data);
            return view;
        }

        public void Return(CardView view)
        {
            view.transform.SetParent(null);
            mPool.Recycle(view);
        }
    }
}
