using System.Collections.Generic;
using Core.Architecture;
using DG.Tweening;
using Features.Card.Data;
using Features.Card.Event;
using Features.Card.Interfaces;
using Features.Card.Model;
using Features.Card.View;
using Features.Combat.Event;
using Features.Combat.System;
using Features.Resource.Model;
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
        [BoxGroup("卡牌布局")]
        [SerializeField] private float hoverPushMax = 80f;
        [BoxGroup("卡牌布局")]
        [SerializeField] private float hoverPushFalloff = 4f;

        [BoxGroup("动画")]
        [SerializeField] private float layoutDuration = 0.15f;
        [BoxGroup("动画")]
        [SerializeField] private float drawStagger = 0.1f;
        [BoxGroup("动画")]
        [SerializeField] private float hoverDuration = 0.18f;

        [BoxGroup("坐标")]
        [SerializeField] private Vector2 drawOrigin = new(1110, -400);
        [BoxGroup("坐标")]
        [SerializeField] private Vector2 discardOrigin = new(-900, -400);

        private ICardViewPool mCardPool;
        private readonly List<CardView> mCardOrder = new();
        private readonly Dictionary<CardData, CardView> mCardLookup = new();
        private readonly HashSet<CardData> mCurrentSet = new();
        private readonly HashSet<CardData> mAddedSet = new();
        private readonly List<Vector2> mCardPositions = new();
        private readonly List<float> mCardAngles = new();
        private int mHoveredIndex = -1;
        private float mLayoutRootBaseY;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public float HoverCardY { get => hoverCardY; }

        public void ForceRefreshLayout()
        {
            int count = mCardOrder.Count;
            CalculatePositions(count, -1);

            for (int i = 0; i < count; i++)
            {
                RectTransform rect = mCardOrder[i].RectTransform;
                rect.DOKill();
                rect.DOAnchorPos(mCardPositions[i], layoutDuration).SetEase(Ease.OutCubic);
                rect.DOLocalRotate(new Vector3(0f, 0f, mCardAngles[i]), layoutDuration).SetEase(Ease.OutCubic);
                rect.SetSiblingIndex(i);
            }
        }

        public void ForceClearHover()
        {
            if (mHoveredIndex < 0)
                return;

            this.GetUtility<ICardHoverDisplay>()?.Hide();

            foreach (CardView card in mCardOrder)
            {
                if (card.CanvasGroup != null)
                    card.CanvasGroup.alpha = 1f;
            }

            mHoveredIndex = -1;
            RefreshLayoutForHover();
        }

        public void ForceEndAllDrags()
        {
            foreach (CardView card in mCardOrder)
            {
                if (card.HandDragHandler.IsDragging)
                    card.HandDragHandler.ForceEndDrag();
            }
        }

        private void Start()
        {
            mCardPool = GetArchitecture().GetUtility<ICardViewPool>();
            mLayoutRootBaseY = LayoutRoot.anchoredPosition.y;

            ICardModel model = this.GetModel<ICardModel>();
            model.OnHandPileChanged.Register(OnHandPileChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<HandCardCostChangedEvent>(OnHandCardCostChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<EnemyTurnStartEvent>(_ => OnEnemyTurnStart())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<PlayerTurnStartEvent>(_ => OnPlayerTurnStart())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<ForceClearHoverEvent>(_ => ForceClearHover())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<ForceEndAllDragsEvent>(_ => ForceEndAllDrags())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<BattleEndCleanupEvent>(_ => OnBattleEnd())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            OnHandPileChanged();
        }

        public void OnCardHovered(int index)
        {
            if (mHoveredIndex == index)
                return;

            SnapToBaseLayout();

            mHoveredIndex = index;
            RefreshLayoutForHover();
        }

        public void OnCardUnhovered(int index)
        {
            if (mHoveredIndex != index)
                return;

            mHoveredIndex = -1;
            RefreshLayoutForHover();
        }

        private void OnHandPileChanged()
        {
            SyncViews(this.GetModel<ICardModel>().HandPile);
        }

        private void OnEnemyTurnStart()
        {
            LayoutRoot.DOAnchorPosY(-90f, 0.3f).SetEase(Ease.OutCubic);
            SetCardOutlines(false);
        }

        private void OnPlayerTurnStart()
        {
            LayoutRoot.DOAnchorPosY(mLayoutRootBaseY, 0.3f).SetEase(Ease.OutCubic);
            SetCardOutlines(true);
        }

        private void OnBattleEnd()
        {
            LayoutRoot.DOKill();
            LayoutRoot.DOAnchorPosY(-300f, 0.2f).SetEase(Ease.InCubic);

            foreach (CardView card in mCardOrder)
                card.RectTransform.DOKill();
        }

        private void SetCardOutlines(bool visible)
        {
            int curEnergy = visible ? this.GetModel<IResourceModel>().CurEnergy.Value : int.MaxValue;
            foreach (CardView card in mCardOrder)
            {
                if (card.OutlineImage != null)
                {
                    bool show = visible && card.CardData != null && card.CardData.Cost <= curEnergy;
                    card.OutlineImage.gameObject.SetActive(show);
                }
            }
        }

        private void OnHandCardCostChanged(HandCardCostChangedEvent @event)
        {
            foreach (CardView card in mCardOrder)
                card.RefreshCost();
            if (this.GetSystem<ITurnSystem>().IsPlayerTurn)
                SetCardOutlines(true);
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

            if (mHoveredIndex >= mCardOrder.Count)
                mHoveredIndex = -1;

            foreach (CardData data in handPile)
            {
                if (!mCardLookup.ContainsKey(data))
                {
                    CardView card = mCardPool.Get(data, LayoutRoot);
                    card.CardHoverHandler.RegisterHandPanel(this, mCardOrder.Count);
                    mCardLookup.Add(data, card);
                    mCardOrder.Add(card);
                    mAddedSet.Add(data);
                }
            }

            for (int i = 0; i < mCardOrder.Count; i++)
                mCardOrder[i].CardHoverHandler.SetHandIndex(i);

            SetCardLayout(mAddedSet);
            if (this.GetSystem<ITurnSystem>().IsPlayerTurn)
                SetCardOutlines(true);
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
            CalculatePositions(count, mHoveredIndex);

            if (newCards.Count > 0)
            {
                this.GetSystem<IInteractionSystem>().BeginAnimation();
                float maxDelay = (count - 1) * drawStagger + layoutDuration;
                DOVirtual.DelayedCall(maxDelay, () => this.GetSystem<IInteractionSystem>().EndAnimation());
            }

            for (int i = 0; i < count; i++)
            {
                CardView card = mCardOrder[i];
                RectTransform rect = card.RectTransform;

                rect.DOKill();
                bool isNew = newCards.Contains(card.CardData);
                float delay = isNew ? i * drawStagger : 0f;

                if (isNew)
                {
                    rect.anchoredPosition = drawOrigin;
                    rect.DOAnchorPos(mCardPositions[i], layoutDuration)
                        .SetEase(Ease.OutCubic)
                        .SetDelay(delay);
                }
                else
                {
                    rect.DOAnchorPos(mCardPositions[i], layoutDuration).SetEase(Ease.OutCubic);
                }

                rect.DOLocalRotate(new Vector3(0f, 0f, mCardAngles[i]), layoutDuration).SetEase(Ease.OutCubic);
                rect.SetSiblingIndex(i);
            }
        }

        private void SnapToBaseLayout()
        {
            int count = mCardOrder.Count;
            CalculatePositions(count, -1);

            for (int i = 0; i < count; i++)
            {
                RectTransform rect = mCardOrder[i].RectTransform;
                rect.DOKill();
                rect.anchoredPosition = mCardPositions[i];
            }
        }

        private void RefreshLayoutForHover()
        {
            int count = mCardOrder.Count;
            CalculatePositions(count, mHoveredIndex);

            for (int i = 0; i < count; i++)
            {
                RectTransform rect = mCardOrder[i].RectTransform;
                rect.DOAnchorPos(mCardPositions[i], hoverDuration).SetEase(Ease.OutCubic);
            }
        }

        private void CalculatePositions(int total, int hoveredIndex)
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

                Vector2 pos = new(
                    -Mathf.Sin(rad) * radius,
                    centerPointY + Mathf.Cos(rad) * radius
                );

                if (hoveredIndex >= 0 && i != hoveredIndex)
                {
                    float dist = Mathf.Abs(hoveredIndex - i);
                    float pushAmount = Mathf.Lerp(hoverPushMax, 0f, Mathf.Min(1f, dist / hoverPushFalloff));
                    pos.x += Mathf.Sign(i - hoveredIndex) * pushAmount;
                }

                mCardPositions.Add(pos);
                mCardAngles.Add(cardAngle);
            }
        }
    }
}