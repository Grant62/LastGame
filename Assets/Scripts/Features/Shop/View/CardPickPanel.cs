using System;
using System.Collections.Generic;
using Core.Architecture;
using Core.Infrastructure;
using DG.Tweening;
using Features.Card.Data;
using Features.Card.Utility;
using Features.Card.View;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Shop.View
{
    public class CardPickPanelData
    {
        public List<CardData> Candidates;
        public int PickCount;
        public int PackPrice;
        public Action<List<CardData>> OnConfirmed;
        public Action OnCancelled;
    }

    public class CardPickPanel : MonoBehaviour, IController
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Transform cardContainer;
        [SerializeField] private Button skipBtn;
        [SerializeField] private Button selectBtnPrefab;
        [SerializeField] private GameObject blockBg;
        [SerializeField] private float selectedOffsetY = GameConstants.CardSelectOffsetY;

        private ICardViewPool mCardPool;
        private readonly List<CardView> mCardViews = new();
        private readonly List<CardData> mConfirmed = new();
        private readonly Dictionary<CardView, Button> mSelectButtons = new();
        private CardView mHoveredCard;
        private CardPickPanelData mData;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            skipBtn.onClick.AddListener(OnSkip);
            canvas.enabled = false;
        }

        public void Show(CardPickPanelData data)
        {
            blockBg.SetActive(true);
            canvas.enabled = true;
            mData = data;
            mConfirmed.Clear();
            mHoveredCard = null;
            LayoutCards(data.Candidates);
        }

        private void LayoutCards(List<CardData> cards)
        {
            if (mCardPool == null)
                mCardPool = this.GetUtility<ICardViewPool>();

            ClearCards();

            foreach (CardData cardData in cards)
            {
                CardView card = mCardPool.Get(cardData, cardContainer, false);
                card.HandDragHandler.enabled = false;
                card.CardHoverHandler.enabled = false;
                card.RectTransform.localEulerAngles = Vector3.zero;

                Button btn = card.GetComponent<Button>() ?? card.gameObject.AddComponent<Button>();
                CardView capturedCard = card;
                CardData capturedData = cardData;
                btn.onClick.AddListener(() => OnCardClicked(capturedCard, capturedData));

                Button selectBtn = Instantiate(selectBtnPrefab, card.RectTransform);
                RectTransform selectRect = (RectTransform)selectBtn.transform;
                selectRect.anchorMin = new Vector2(0.5f, 0f);
                selectRect.anchorMax = new Vector2(0.5f, 0f);
                selectRect.pivot = new Vector2(0.5f, 1f);
                selectRect.anchoredPosition = new Vector2(0, GameConstants.CardActionButtonOffsetY);
                selectBtn.onClick.AddListener(() => OnSelectConfirmed(capturedCard, capturedData));
                selectBtn.gameObject.SetActive(false);

                mSelectButtons[card] = selectBtn;
                mCardViews.Add(card);
            }
        }

        private void OnCardClicked(CardView card, CardData data)
        {
            if (mConfirmed.Contains(data))
                return;

            if (mHoveredCard != null && mHoveredCard != card)
            {
                float prevY = mHoveredCard.RectTransform.anchoredPosition.y;
                mHoveredCard.RectTransform.DOAnchorPosY(prevY - selectedOffsetY, 0.15f);
                if (mSelectButtons.TryGetValue(mHoveredCard, out Button prevBtn))
                    prevBtn.gameObject.SetActive(false);
            }

            if (mHoveredCard == card)
            {
                float curY = card.RectTransform.anchoredPosition.y;
                card.RectTransform.DOAnchorPosY(curY - selectedOffsetY, 0.15f);
                if (mSelectButtons.TryGetValue(card, out Button selfBtn))
                    selfBtn.gameObject.SetActive(false);
                mHoveredCard = null;
            }
            else
            {
                float curY = card.RectTransform.anchoredPosition.y;
                card.RectTransform.DOAnchorPosY(curY + selectedOffsetY, 0.15f);
                if (mSelectButtons.TryGetValue(card, out Button selfBtn))
                    selfBtn.gameObject.SetActive(true);
                mHoveredCard = card;
            }
        }

        private void OnSelectConfirmed(CardView card, CardData data)
        {
            mConfirmed.Add(data);
            mHoveredCard = null;
            RecycleCardView(card);

            if (mConfirmed.Count >= mData.PickCount)
            {
                mData.OnConfirmed.Invoke(new List<CardData>(mConfirmed));
                Hide();
            }
        }

        private void OnSkip()
        {
            if (mConfirmed.Count > 0)
                mData.OnConfirmed.Invoke(new List<CardData>(mConfirmed));
            else
                mData.OnCancelled.Invoke();

            Hide();
        }

        private void RecycleCardView(CardView card)
        {
            card.RectTransform.DOKill();

            Destroy(card.gameObject.GetComponent<Button>());

            if (mSelectButtons.TryGetValue(card, out Button selectBtn))
            {
                selectBtn.onClick.RemoveAllListeners();
                Destroy(selectBtn.gameObject);
            }

            mSelectButtons.Remove(card);
            mCardViews.Remove(card);
            mCardPool.Return(card);
        }

        private void Hide()
        {
            ClearCards();
            mSelectButtons.Clear();
            mHoveredCard = null;
            canvas.enabled = false;
        }

        private void ClearCards()
        {
            foreach (CardView card in mCardViews)
            {
                card.RectTransform.DOKill();
                Destroy(card.gameObject.GetComponent<Button>());

                if (mSelectButtons.TryGetValue(card, out Button selectBtn))
                {
                    selectBtn.onClick.RemoveAllListeners();
                    Destroy(selectBtn.gameObject);
                }

                mCardPool.Return(card);
            }

            mCardViews.Clear();
            mConfirmed.Clear();
        }

        private void OnDestroy()
        {
            skipBtn.onClick.RemoveListener(OnSkip);
        }
    }
}