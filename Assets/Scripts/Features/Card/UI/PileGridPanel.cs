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
    public class PileGridPanelData : UIPanelData
    {
        public List<CardData> Cards;
    }

    public partial class PileGridPanel : UIPanel, IController
    {
        private ICardViewPool mCardPool;
        private readonly List<CardView> mCardViews = new();

        [BoxGroup("滚动")]
        [SerializeField] private float scrollSensitivity = 30f;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mCardPool = this.GetUtility<ICardViewPool>();
            GetComponent<ScrollRect>().scrollSensitivity = scrollSensitivity;
            Close.onClick.AddListener(CloseSelf);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            PileGridPanelData data = uiData as PileGridPanelData;
            if (data?.Cards == null)
                return;

            LayoutCards(data.Cards);
        }

        protected override void OnClose()
        {
            ReleaseCards();
            Close.onClick.RemoveListener(CloseSelf);
        }

        private void LayoutCards(List<CardData> cards)
        {
            ReleaseCards();

            foreach (CardData cardData in cards)
            {
                CardView card = mCardPool.Get(cardData, Content, false);
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
    }
}