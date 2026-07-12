using System;
using System.Collections.Generic;
using Core.Architecture;
using Core.Infrastructure;
using Core.Systems;
using DG.Tweening;
using Features.Card.Data;
using Features.Card.Utility;
using Features.Card.View;
using QFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Features.Shop.View
{
    public class RemoveCardPanel : MonoBehaviour, IController
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject blockBg;
        [SerializeField] private Transform content;
        [SerializeField] private Button deleteBtnPrefab;
        [SerializeField] private Button closeBtn;
        [SerializeField] private float selectedOffsetY = GameConstants.CardSelectOffsetY;

        private readonly List<CardView> mCardViews = new();
        private readonly Dictionary<CardView, Button> mDeleteButtons = new();
        private CardView mHoveredCard;
        private CardData mHoveredData;
        private Action<CardData> mOnConfirm;
        private Action mOnClose;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            canvas.enabled = false;
            closeBtn.onClick.AddListener(OnClose);
        }

        public void Show(List<CardData> cards, Action<CardData> onConfirm, Action onClose = null)
        {
            blockBg.SetActive(true);
            canvas.enabled = true;
            mOnConfirm = onConfirm;
            mOnClose = onClose;
            mHoveredCard = null;
            mHoveredData = null;

            LayoutCards(cards);
            GameMain.Interface.GetSystem<IPopupStackSystem>().Push(gameObject);
        }

        private void LayoutCards(List<CardData> cards)
        {
            ReleaseCards();
            ICardViewPool pool = this.GetUtility<ICardViewPool>();

            foreach (CardData cardData in cards)
            {
                CardView card = pool.Get(cardData, content, false);
                card.HandDragHandler.enabled = false;
                card.CardHoverHandler.enabled = false;
                card.RectTransform.localEulerAngles = Vector3.zero;

                Button btn = card.GetComponent<Button>() ?? card.gameObject.AddComponent<Button>();
                CardView capturedCard = card;
                CardData capturedData = cardData;
                btn.onClick.AddListener(() => OnCardClicked(capturedCard, capturedData));

                mCardViews.Add(card);
            }
        }

        private void OnCardClicked(CardView card, CardData data)
        {
            if (mHoveredCard != null && mHoveredCard != card)
            {
                float prevY = mHoveredCard.RectTransform.anchoredPosition.y;
                mHoveredCard.RectTransform.DOAnchorPosY(prevY - selectedOffsetY, 0.15f);
                HideDeleteButton(mHoveredCard);
            }

            if (mHoveredCard == card)
            {
                float curY = card.RectTransform.anchoredPosition.y;
                card.RectTransform.DOAnchorPosY(curY - selectedOffsetY, 0.15f);
                HideDeleteButton(card);
                mHoveredCard = null;
                mHoveredData = null;
            }
            else
            {
                float curY = card.RectTransform.anchoredPosition.y;
                card.RectTransform.DOAnchorPosY(curY + selectedOffsetY, 0.15f);
                ShowDeleteButton(card);
                mHoveredCard = card;
                mHoveredData = data;
            }
        }

        private void ShowDeleteButton(CardView card)
        {
            Button deleteBtn = Instantiate(deleteBtnPrefab, card.RectTransform);
            RectTransform deleteRect = (RectTransform)deleteBtn.transform;
            deleteRect.anchorMin = new Vector2(0.5f, 0f);
            deleteRect.anchorMax = new Vector2(0.5f, 0f);
            deleteRect.pivot = new Vector2(0.5f, 1f);
            deleteRect.anchoredPosition = new Vector2(0, GameConstants.CardActionButtonOffsetY);

            CardData capturedData = mHoveredData;
            deleteBtn.onClick.AddListener(() => OnDeleteConfirmed(capturedData));

            mDeleteButtons[card] = deleteBtn;
        }

        private void HideDeleteButton(CardView card)
        {
            if (mDeleteButtons.TryGetValue(card, out Button deleteBtn))
            {
                deleteBtn.onClick.RemoveAllListeners();
                Destroy(deleteBtn.gameObject);
                mDeleteButtons.Remove(card);
            }
        }

        private void OnDeleteConfirmed(CardData data)
        {
            mOnConfirm?.Invoke(data);
            Hide();
        }

        private void OnClose()
        {
            mOnClose?.Invoke();
            Hide();
        }

        private void Hide()
        {
            ReleaseCards();
            mHoveredCard = null;
            mHoveredData = null;
            mDeleteButtons.Clear();
            canvas.enabled = false;
            GameMain.Interface.GetSystem<IPopupStackSystem>().Remove(gameObject);
            Addressables.ReleaseInstance(gameObject);
            Destroy(gameObject);
        }

        private void ReleaseCards()
        {
            ICardViewPool pool = this.GetUtility<ICardViewPool>();
            foreach (CardView view in mCardViews)
            {
                view.RectTransform.DOKill();
                Destroy(view.GetComponent<Button>());
                pool.Return(view);
            }

            mCardViews.Clear();

            foreach (Button deleteBtn in mDeleteButtons.Values)
                Destroy(deleteBtn.gameObject);
            mDeleteButtons.Clear();
        }

        private void OnDestroy()
        {
            closeBtn?.onClick.RemoveListener(OnClose);
        }
    }
}