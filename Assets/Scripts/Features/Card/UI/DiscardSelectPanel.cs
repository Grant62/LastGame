using System;
using System.Collections.Generic;
using Core.Architecture;
using Core.Systems;
using Features.Card.Data;
using Features.Card.Model;
using Features.Card.Utility;
using Features.Card.View;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Card.UI
{
    public class DiscardSelectPanelData
    {
        public List<CardData> HandCards;
        public Action<CardData> OnSelected;
    }

    public class DiscardSelectPanel : MonoBehaviour, IController
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

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            mCardPool = this.GetUtility<ICardViewPool>();
            confirmBtn.onClick.AddListener(OnConfirm);
        }

        public void Open(DiscardSelectPanelData data)
        {
            gameObject.SetActive(true);
            mCallback = data.OnSelected;
            tipText.text = "选择 1 张手牌弃掉";
            LayoutCards(data.HandCards);

            scrollRect.verticalScrollbar.gameObject.SetActive(true);
            GameMain.Interface.GetSystem<IPopupStackSystem>().Push(gameObject);
        }

        private void Close()
        {
            ReleaseCards();
            mCallback = null;
            GameMain.Interface.GetSystem<IPopupStackSystem>().Remove(gameObject);
            gameObject.SetActive(false);
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
            List<CardData> handPile = this.GetModel<ICardModel>().HandPile;
            CardData selectedData = index >= 0 && index < handPile.Count ? handPile[index] : null;

            mCallback?.Invoke(selectedData);
            Close();
        }

        private void ReleaseCards()
        {
            foreach (CardView card in mCardViews)
            {
                Destroy(card.GetComponent<Button>());

                card.CardImage.material = null;
                mCardPool.Return(card);
            }

            mCardViews.Clear();
            mSelected = null;
        }
    }
}