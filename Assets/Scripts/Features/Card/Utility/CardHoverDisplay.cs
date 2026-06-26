using DG.Tweening;
using Features.Card.Data;
using Features.Card.Interfaces;
using Features.Card.View;
using UnityEngine;

namespace Features.Card.Utility
{
    public class CardHoverDisplay : ICardHoverDisplay
    {
        private readonly CardView mCardView;
        private readonly IKeywordResolver mKeywordResolver;
        private readonly float mHoverScale = 1.3f;

        public CardHoverDisplay(CardView hoverCard, IKeywordResolver keywordResolver)
        {
            mCardView = hoverCard;
            mKeywordResolver = keywordResolver;

            mCardView.CanvasGroup.blocksRaycasts = false;
            mCardView.HandDragHandler.enabled = false;

            mCardView.gameObject.SetActive(false);
        }

        public void Show(CardData data, Vector3 position)
        {
            mCardView.Setup(data);

            string keywords = mKeywordResolver.GetKeywordExplanations(data.Desc);
            if (!string.IsNullOrEmpty(keywords))
                mCardView.Desc.text = mCardView.Desc.text + "\n\n" + keywords;

            mCardView.transform.position = position;
            mCardView.RectTransform.localScale = Vector3.one * 0.8f;
            mCardView.gameObject.SetActive(true);
            mCardView.RectTransform.DOScale(mHoverScale, 0.12f).SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            mCardView.RectTransform.DOScale(1f, 0.08f).SetEase(Ease.InCubic)
                .OnComplete(() => mCardView.gameObject.SetActive(false));
        }
    }
}