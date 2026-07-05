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
    public partial class PileGridPanel : MonoBehaviour, IController
    {
        private readonly List<CardView> mCardViews = new();

        [BoxGroup("滚动")]
        [SerializeField] private float scrollSensitivity = 30f;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            GetComponent<ScrollRect>().scrollSensitivity = scrollSensitivity;
            Close.onClick.AddListener(OnClose);
        }

        public void Show(List<CardData> cards)
        {
            gameObject.SetActive(true);
            LayoutCards(cards);
        }

        private void OnClose()
        {
            ReleaseCards();
            gameObject.SetActive(false);
        }

        private void LayoutCards(List<CardData> cards)
        {
            ReleaseCards();

            foreach (CardData cardData in cards)
            {
                CardView card = this.GetUtility<ICardViewPool>().Get(cardData, Content, false);
                card.HandDragHandler.enabled = false;
                card.CardHoverHandler.enabled = false;
                card.RectTransform.localEulerAngles = Vector3.zero;
                mCardViews.Add(card);
            }
        }

        private void ReleaseCards()
        {
            ICardViewPool pool = this.GetUtility<ICardViewPool>();
            foreach (CardView view in mCardViews)
                pool.Return(view);
            mCardViews.Clear();
        }
    }
}