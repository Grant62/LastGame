using System.Collections.Generic;
using Core.Architecture;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Features.Card.Data;
using Features.Card.Event;
using Features.Card.Interfaces;
using Features.Card.Model;
using Features.Card.View;
using Features.Combat.System;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Card.UI
{
    public partial class HandPanel : MonoBehaviour, IController, IHoverContext
    {
        [BoxGroup("卡牌布局")]
        [SerializeField] private float maxTotalAngle = 35f;
        [BoxGroup("卡牌布局")]
        [SerializeField] private float angleBetweenCards = 3.5f;
        [BoxGroup("卡牌布局")]
        [SerializeField] private float radius = 3000f;
        [BoxGroup("卡牌布局")]
        [SerializeField] private float centerPointY = -3380f;
        [BoxGroup("卡牌布局")]
        [SerializeField] private float hoverCardY = 190f;

        [BoxGroup("动画")]
        [SerializeField] private float layoutDuration = 0.15f;
        [BoxGroup("动画")]
        [SerializeField] private float drawStagger = 0.1f;

        [BoxGroup("坐标")]
        [SerializeField] private Vector2 drawOrigin = new(1110, -400);
        [BoxGroup("坐标")]
        [SerializeField] private Vector2 discardOrigin = new(-900, -400);

        [BoxGroup("引用")]
        [SerializeField] private Canvas overlayCanvas;

        private ICardViewPool mCardPool;
        private readonly List<CardView> mCardOrder = new();
        private readonly Dictionary<CardData, CardView> mCardLookup = new();
        private readonly HashSet<CardData> mCurrentSet = new();
        private readonly HashSet<CardData> mAddedSet = new();
        private readonly List<Vector2> mCardPositions = new();
        private readonly List<float> mCardAngles = new();

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public float HoverCardY { get => hoverCardY; }

        public Canvas OverlayCanvas { get => overlayCanvas; }

        private void Start()
        {
            mCardPool = GetArchitecture().GetUtility<ICardViewPool>();

            ICardModel model = this.GetModel<ICardModel>();
            model.OnHandPileChanged.Register(OnHandPileChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<HandCardCostChangedEvent>(OnHandCardCostChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            OnHandPileChanged();
        }

        private void OnHandPileChanged()
        {
            SyncViews(this.GetModel<ICardModel>().HandPile);
        }

        private void OnHandCardCostChanged(HandCardCostChangedEvent e)
        {
            foreach (CardView card in mCardOrder)
                card.RefreshCost();
        }

        private void SyncViews(List<CardData> handPile)
        {
            mCurrentSet.Clear();
            foreach (CardData d in handPile) mCurrentSet.Add(d);
            mAddedSet.Clear();

            int discardIndex = 0;
            for (int i = mCardOrder.Count - 1; i >= 0; i--)
            {
                CardView card = mCardOrder[i];
                if (!mCurrentSet.Contains(card.CardData))
                {
                    mCardLookup.Remove(card.CardData);
                    mCardOrder.RemoveAt(i);
                    float delay = discardIndex * drawStagger;
                    AnimateDiscard(card, delay);
                    discardIndex++;
                }
            }

            foreach (CardData data in handPile)
            {
                if (!mCardLookup.ContainsKey(data))
                {
                    CardView card = mCardPool.Get(data, LayoutRoot);
                    mCardLookup.Add(data, card);
                    mCardOrder.Add(card);
                    mAddedSet.Add(data);
                }
            }

            SetCardLayout(mAddedSet);
        }

        private void AnimateDiscard(CardView card, float delay)
        {
            RectTransform rect = card.RectTransform;
            CanvasGroup cg = card.CanvasGroup;
            rect.DOKill();
            rect.DOAnchorPos(discardOrigin, layoutDuration).SetEase(Ease.InCubic).SetDelay(delay);
            cg.DOFade(0f, layoutDuration).SetDelay(delay);

            DOVirtual.DelayedCall(layoutDuration + delay, () => mCardPool.Return(card));
        }

        private void SetCardLayout(HashSet<CardData> newCards)
        {
            int count = mCardOrder.Count;
            CalculatePositions(count);

            if (newCards.Count > 0)
                this.GetSystem<IInteractionSystem>().BeginAnimation();

            for (int i = 0; i < count; i++)
            {
                CardView card = mCardOrder[i];
                RectTransform rect = card.RectTransform;

                rect.DOKill();
                bool isNew = newCards.Contains(card.CardData);
                float delay = isNew ? i * drawStagger : 0f;
                bool isLast = i == count - 1;

                if (isNew)
                {
                    rect.anchoredPosition = drawOrigin;
                    TweenerCore<Vector2, Vector2, VectorOptions> tween = rect.DOAnchorPos(mCardPositions[i], layoutDuration)
                        .SetEase(Ease.OutCubic)
                        .SetDelay(delay);
                    if (isLast)
                        tween.OnComplete(() => this.GetSystem<IInteractionSystem>().EndAnimation());
                }
                else
                {
                    rect.DOAnchorPos(mCardPositions[i], layoutDuration).SetEase(Ease.OutCubic);
                }

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