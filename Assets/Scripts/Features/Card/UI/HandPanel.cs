using System.Collections.Generic;
using Core.Architecture;
using DG.Tweening;
using Features.Card.Data;
using Features.Card.Interfaces;
using Features.Card.Model;
using QFramework;
using UnityEngine;

namespace Features.Card.UI
{
    public partial class HandPanel : ViewController, IController
    {
        [SerializeField] private float maxTotalAngle = 26f;
        [SerializeField] private float angleBetweenCards = 4f;
        [SerializeField] private float radius = 400f;
        [SerializeField] private float centerPointY = -150f;
        [SerializeField] private float layoutDuration = 0.15f;

        private ICardUIPool mCardPool;
        private readonly List<CardUI> mCardOrder = new();
        private readonly Dictionary<CardData, CardUI> mCardLookup = new();
        private readonly List<Vector2> mCardPositions = new();
        private readonly List<float> mCardAngles = new();

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            mCardPool = GameMain.Interface.GetUtility<ICardUIPool>();

            ICardModel model = this.GetModel<ICardModel>();
            model.OnHandPileChanged.Register(OnHandPileChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            OnHandPileChanged();
        }

        private void OnHandPileChanged()
        {
            SyncViews(this.GetModel<ICardModel>().HandPile);
            SetCardLayout();
        }

        private void SyncViews(List<CardData> handPile)
        {
            HashSet<CardData> current = new(handPile);

            for (int i = mCardOrder.Count - 1; i >= 0; i--)
            {
                CardUI card = mCardOrder[i];
                if (!current.Contains(card.CardData))
                {
                    mCardLookup.Remove(card.CardData);
                    mCardOrder.RemoveAt(i);
                    mCardPool.Return(card);
                }
            }

            foreach (CardData data in handPile)
            {
                if (!mCardLookup.ContainsKey(data))
                {
                    CardUI card = mCardPool.Get(data, LayoutRoot);
                    mCardLookup.Add(data, card);
                    mCardOrder.Add(card);
                }
            }
        }

        private void SetCardLayout()
        {
            int count = mCardOrder.Count;
            CalculatePositions(count);

            for (int i = 0; i < count; i++)
            {
                CardUI card = mCardOrder[i];
                RectTransform rect = card.GetComponent<RectTransform>();
                if (rect == null)
                    continue;

                rect.DOKill();
                rect.DOAnchorPos(mCardPositions[i], layoutDuration).SetEase(Ease.OutCubic);
                rect.DOLocalRotate(new Vector3(0f, 0f, mCardAngles[i]), layoutDuration).SetEase(Ease.OutCubic);
                rect.SetSiblingIndex(i);
            }
        }

        private void CalculatePositions(int total)
        {
            mCardPositions.Clear();
            mCardAngles.Clear();

            if (total == 0)
                return;

            float currentTotalAngle = (total - 1) * angleBetweenCards;
            float totalAngle = Mathf.Min(maxTotalAngle, currentTotalAngle);
            float currentSpacing = total > 1 ? totalAngle / (total - 1) : angleBetweenCards;

            for (int i = 0; i < total; i++)
            {
                float cardAngle = totalAngle / 2f - i * currentSpacing;
                float rad = cardAngle * Mathf.Deg2Rad;

                mCardPositions.Add(new Vector2(
                    -Mathf.Sin(rad) * radius,
                    centerPointY + Mathf.Cos(rad) * radius
                ));
                mCardAngles.Add(cardAngle);
            }
        }
    }
}