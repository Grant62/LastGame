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

        public CardViewPool(CardView prefab)
        {
            mPool = new SimpleObjectPool<CardView>(
                () => Object.Instantiate(prefab),
                null,
                5
            );
        }

        public CardView Get(CardData data, Transform parent)
        {
            CardView card = mPool.Allocate();
            card.transform.SetParent(parent, false);
            card.Setup(data);
            return card;
        }

        public void Return(CardView view)
        {
            view.Reset();
            view.transform.SetParent(null, false);
            mPool.Recycle(view);
        }
    }
}