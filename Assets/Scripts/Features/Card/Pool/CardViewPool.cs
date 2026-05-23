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
        private readonly Vector3 mPrefabScale;
        private readonly SimpleObjectPool<CardView> mPool;

        public CardViewPool(CardView prefab)
        {
            mPrefab = prefab;
            mPrefabScale = prefab.transform.localScale;
            mPool = new SimpleObjectPool<CardView>(
                () => Object.Instantiate(mPrefab),
                view => view.gameObject.SetActive(false)
            );
        }

        public CardView Get(CardData data, Transform parent)
        {
            CardView view = mPool.Allocate();
            view.transform.SetParent(parent);
            view.transform.localScale = mPrefabScale;
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