using Features.Card.Data;
using Features.Card.Interfaces;
using Features.Card.UI;
using QFramework;
using UnityEngine;

namespace Features.Card.Pool
{
    public class CardUIPool : ICardUIPool
    {
        private readonly SimpleObjectPool<CardUI> mPool;

        public CardUIPool(CardUI prefab)
        {
            mPool = new SimpleObjectPool<CardUI>(
                () => Object.Instantiate(prefab),
                null,
                5
            );
        }

        public CardUI Get(CardData data, Transform parent)
        {
            CardUI card = mPool.Allocate();
            card.transform.SetParent(parent, false);
            card.Setup(data);
            return card;
        }

        public void Return(CardUI view)
        {
            view.Reset();
            view.transform.SetParent(null, false);
            mPool.Recycle(view);
        }
    }
}