using System.Collections.Generic;
using Core.Architecture;
using Features.Card.Data;
using Features.Card.Interfaces;
using QFramework;

namespace Features.Card.UI
{
    public partial class PileGridPanel : ViewController, IController
    {
        private ICardUIPool mCardPool;
        private List<CardUI> mCardViews;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            mCardViews = new List<CardUI>();
            mCardPool = GameMain.Interface.GetUtility<ICardUIPool>();
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
                CardUI card = mCardPool.Get(data, Content);
                HandDragHandler handler = card.GetComponent<HandDragHandler>();
                handler.enabled = false;

                mCardViews.Add(card);
            }
        }

        private void ReleaseCards()
        {
            foreach (CardUI view in mCardViews)
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