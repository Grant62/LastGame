using System.Collections.Generic;
using Core.Architecture;
using Features.Card.Data;
using Features.Card.Interfaces;
using Features.Card.View;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Card.UI
{
    public partial class PileGridPanel : ViewController, IController
    {
        private ICardViewPool mCardPool;
        private List<CardView> mCardViews;

        [BoxGroup("滚动")]
        [SerializeField] private float scrollSensitivity = 30f;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            mCardViews = new List<CardView>();
            mCardPool = GetArchitecture().GetUtility<ICardViewPool>();
            GetComponent<ScrollRect>().scrollSensitivity = scrollSensitivity;
            Close.onClick.AddListener(OnCloseClicked);
        }

        public void Open(List<CardData> cards)
        {
            if (gameObject.activeSelf)
                return;

            gameObject.SetActive(true);
            LayoutCards(cards);
        }

        public void ClosePanel()
        {
            if (!gameObject.activeSelf)
                return;

            ReleaseCards();
            gameObject.SetActive(false);
        }

        private void OnCloseClicked()
        {
            ClosePanel();
        }

        private void LayoutCards(List<CardData> cards)
        {
            ReleaseCards();

            foreach (CardData data in cards)
            {
                CardView card = mCardPool.Get(data, Content, false);
                card.HandDragHandler.enabled = false;
                card.CardHoverHandler.enabled = false;
                card.RectTransform.localEulerAngles = Vector3.zero;

                mCardViews.Add(card);
            }
        }

        private void ReleaseCards()
        {
            foreach (CardView view in mCardViews)
                mCardPool.Return(view);

            mCardViews.Clear();
        }

        private void OnDestroy()
        {
            ReleaseCards();
            Close.onClick.RemoveListener(OnCloseClicked);
        }
    }
}