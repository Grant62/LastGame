using System;
using System.Collections.Generic;
using Core.Architecture;
using Features.Card.Data;
using Features.Card.Interfaces;
using Features.Card.Model;
using Features.Card.View;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Card.UI
{
    public class DiscardSelectPanelData : UIPanelData
    {
        public List<CardData> HandCards;
        public Action<CardData> OnSelected;
    }

    public class DiscardSelectPanel : UIPanel
    {
        [SerializeField] private Transform content;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Button confirmBtn;
        [SerializeField] private TMP_Text tipText;
        [SerializeField] private Material outlineMat;

        private ICardViewPool mCardPool;
        private readonly List<CardView> mCardViews = new();
        private CardView mSelected;
        private Action<CardData> mCallback;

        protected override void OnInit(IUIData uiData = null)
        {
            mCardPool = GameMain.Interface.GetUtility<ICardViewPool>();
            confirmBtn.onClick.AddListener(OnConfirm);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            DiscardSelectPanelData data = uiData as DiscardSelectPanelData ?? throw new ArgumentNullException(nameof(uiData));
            mCallback = data.OnSelected;
            tipText.text = "选择 1 张手牌弃掉";
            LayoutCards(data.HandCards);

            if (scrollRect != null && scrollRect.verticalScrollbar != null)
                scrollRect.verticalScrollbar.gameObject.SetActive(true);
        }

        protected override void OnClose()
        {
            ReleaseCards();
            mCallback = null;
        }

        private void LayoutCards(List<CardData> handCards)
        {
            foreach (CardData cardData in handCards)
            {
                CardView card = mCardPool.Get(cardData, content, false);
                card.RectTransform.localScale = new Vector3(1.2f, 1.2f, 1f);
                card.RectTransform.localEulerAngles = Vector3.zero;
                card.HandDragHandler.enabled = false;
                card.CardHoverHandler.enabled = false;

                Button btn = card.GetComponent<Button>();
                if (btn == null)
                    btn = card.gameObject.AddComponent<Button>();

                CardData captured = cardData;
                btn.onClick.AddListener(() => OnCardClicked(card, captured));
                mCardViews.Add(card);
            }
        }

        private void OnCardClicked(CardView card, CardData cardData)
        {
            if (mSelected != null && mSelected != card)
            {
                mSelected.CardImage.material = null;
                mSelected.RectTransform.localScale = new Vector3(1.2f, 1.2f, 1f);
            }

            if (mSelected == card)
            {
                mSelected = null;
                card.CardImage.material = null;
                card.RectTransform.localScale = new Vector3(1.2f, 1.2f, 1f);
                confirmBtn.interactable = false;
                return;
            }

            mSelected = card;
            card.CardImage.material = outlineMat;
            confirmBtn.interactable = true;
        }

        private void OnConfirm()
        {
            if (mSelected == null)
                return;

            int index = mCardViews.IndexOf(mSelected);
            List<CardData> handPile = GameMain.Interface.GetModel<ICardModel>().HandPile;
            CardData selectedData = index >= 0 && index < handPile.Count ? handPile[index] : null;

            mCallback?.Invoke(selectedData);
            CloseSelf();
        }

        private void ReleaseCards()
        {
            foreach (CardView card in mCardViews)
            {
                Button btn = card.GetComponent<Button>();
                if (btn != null)
                    Destroy(btn);

                card.CardImage.material = null;
                mCardPool.Return(card);
            }

            mCardViews.Clear();
            mSelected = null;
        }
    }
}