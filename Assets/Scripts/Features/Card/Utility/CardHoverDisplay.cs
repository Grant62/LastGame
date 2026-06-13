using Features.Card.Data;
using Features.Card.Interfaces;
using Features.Card.View;
using UnityEngine;

namespace Features.Card.Utility
{
    public class CardHoverDisplay : ICardHoverDisplay
    {
        private readonly CardView mCardView;

        public CardHoverDisplay(CardView hoverCard)
        {
            mCardView = hoverCard;

            CanvasGroup cg = mCardView.GetComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            mCardView.GetComponent<CardHoverHandler>().enabled = false;
            mCardView.GetComponent<HandDragHandler>().enabled = false;

            mCardView.gameObject.SetActive(false);
        }

        public void Show(CardData data, Vector3 position)
        {
            mCardView.Setup(data);
            mCardView.transform.position = position;
            mCardView.gameObject.SetActive(true);
        }

        public void Hide()
        {
            mCardView.gameObject.SetActive(false);
        }
    }
}