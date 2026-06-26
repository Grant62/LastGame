using Features.Card.Data;
using Features.Card.Interfaces;
using Features.Card.View;
using QFramework;
using UnityEngine;

namespace Features.Card.Utility
{
    public class CardViewPool : ICardViewPool
    {
        private readonly SimpleObjectPool<CardView> mPool;
        private readonly Transform mPoolRoot;

        public CardViewPool(CardView prefab)
        {
            mPool = new SimpleObjectPool<CardView>(
                () => Object.Instantiate(prefab),
                null,
                5
            );

            GameObject go = new("[Pool] CardView");
            go.SetActive(false);
            mPoolRoot = go.transform;
        }

        public CardView Get(CardData data, Transform parent, bool enableEffects = true)
        {
            CardView card = mPool.Allocate();
            card.transform.SetParent(parent, false);
            card.InitFromParent();
            card.Setup(data, enableEffects);
            return card;
        }

        public void Return(CardView view)
        {
            view.Reset();
            view.transform.SetParent(mPoolRoot, false);
            mPool.Recycle(view);
        }
    }
}