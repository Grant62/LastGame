using Features.Card.Data;
using Features.Card.Interfaces;
using UnityEngine;

namespace Features.Card.UI
{
    public class CardHoverDisplay : ICardHoverDisplay
    {
        private readonly CardUI mCardView;
        private readonly float mHoverOffset;

        public CardHoverDisplay(CardUI hoverCard, float hoverOffset)
        {
            mCardView = hoverCard;
            mHoverOffset = hoverOffset;

            CanvasGroup cg = mCardView.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = mCardView.gameObject.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            CardHoverHandler hover = mCardView.GetComponent<CardHoverHandler>();
            if (hover != null)
                hover.enabled = false;

            HandDragHandler drag = mCardView.GetComponent<HandDragHandler>();
            if (drag != null)
                drag.enabled = false;

            mCardView.gameObject.SetActive(false);
        }

        public void Show(CardData data, Vector3 position)
        {
            mCardView.Setup(data);
            mCardView.transform.position = position + Vector3.up * mHoverOffset;
            mCardView.gameObject.SetActive(true);
        }

        public void Hide()
        {
            mCardView.gameObject.SetActive(false);
        }
    }
}