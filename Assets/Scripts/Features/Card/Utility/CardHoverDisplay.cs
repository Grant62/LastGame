using System.Collections.Generic;
using Core.Architecture;
using DG.Tweening;
using Features.Card.Data;
using Features.Card.View;
using UnityEngine;

namespace Features.Card.Utility
{
    public class CardHoverDisplay : ICardHoverDisplay
    {
        private readonly CardView mCardView;
        private readonly IKeywordResolver mKeywordResolver;
        private readonly IKeywordCardPool mPool;
        private readonly float mHoverScale = 1.3f;
        private readonly List<KeywordCard> mKeywordCards = new();

        private const float CardWidth = 260f;
        private const float CardSpacing = 9f;
        private const float ColumnGap = 9f;
        private const float ColumnOffsetX = 140f;
        private const float StartOffsetY = 200f;
        private const float DescWidth = 260f;
        private const float CardMinHeight = 60f;

        public CardHoverDisplay(CardView hoverCard, IKeywordResolver keywordResolver, IKeywordCardPool pool)
        {
            mCardView = hoverCard;
            mKeywordResolver = keywordResolver;
            mPool = pool;

            mCardView.CanvasGroup.blocksRaycasts = false;
            mCardView.HandDragHandler.enabled = false;

            mCardView.gameObject.SetActive(false);
        }

        public void Show(CardData data, Vector3 position)
        {
            mCardView.Setup(data);

            List<(string name, string desc)> keywords = mKeywordResolver.CollectKeywords(data.Desc);
            float curX = position.x + mCardView.RectTransform.rect.width * mHoverScale * 0.5f + ColumnOffsetX;
            float curTop = position.y + StartOffsetY;
            float columnTop = curTop;

            for (int i = 0; i < keywords.Count; i++)
            {
                (string name, string desc) = keywords[i];
                desc = mKeywordResolver.FormatDescription(desc.Replace("\\n", "\n"));

                KeywordCard card = mPool.Get(GameRoot.CombatOverlay);
                card.Setup(name, desc);
                mKeywordCards.Add(card);

                Vector2 preferred = card.DescText.GetPreferredValues(card.DescText.text, DescWidth, 0f);
                float descHeight = Mathf.Max(preferred.y, 24f);
                float cardHeight = 24f + descHeight;
                cardHeight = Mathf.Max(cardHeight, CardMinHeight);

                RectTransform rt = (RectTransform)card.transform;
                RectTransform descRt = card.DescRectTransform;
                descRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, descHeight);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cardHeight);

                if (curTop - cardHeight < 0)
                {
                    curX += CardWidth + ColumnGap;
                    curTop = columnTop;
                }

                float centerY = curTop - cardHeight * 0.5f;
                card.transform.position = new Vector3(curX, centerY, 0f);
                curTop -= cardHeight + CardSpacing;
            }

            mCardView.transform.position = position;
            mCardView.RectTransform.localScale = Vector3.one * 0.8f;
            mCardView.gameObject.SetActive(true);
            mCardView.RectTransform.DOScale(mHoverScale, 0.12f).SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            foreach (KeywordCard card in mKeywordCards)
                mPool.Return(card);
            mKeywordCards.Clear();

            mCardView.RectTransform.DOScale(1f, 0.08f).SetEase(Ease.InCubic)
                .SetUpdate(true)
                .OnComplete(() => mCardView.gameObject.SetActive(false));
        }
    }
}
