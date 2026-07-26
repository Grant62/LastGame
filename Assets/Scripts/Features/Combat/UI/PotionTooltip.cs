using System.Collections.Generic;
using Core.Architecture;
using Features.Card.Utility;
using Features.Card.View;
using UnityEngine;

namespace Features.Combat.UI
{
    public class PotionTooltip : IPotionTooltip
    {
        private readonly IKeywordResolver mKeywordResolver;
        private readonly IKeywordCardPool mPool;
        private readonly List<KeywordCard> mCards = new();

        private const float CardWidth = 260f;
        private const float CardSpacing = 9f;
        private const float ColumnGap = 9f;
        private const float ColumnOffsetX = 140f;
        private const float StartOffsetY = 200f;
        private const float DescWidth = 260f;
        private const float CardMinHeight = 60f;

        public PotionTooltip(IKeywordResolver keywordResolver, IKeywordCardPool pool)
        {
            mKeywordResolver = keywordResolver;
            mPool = pool;
        }

        public void Show(cfg.PotionInfo potion, Vector3 position)
        {
            Hide();

            string formattedDesc = mKeywordResolver.FormatDescription(potion.Desc.Replace("\\n", "\n"));

            List<(string name, string desc)> allCards = new()
            {
                (potion.Name, formattedDesc)
            };
            allCards.AddRange(mKeywordResolver.CollectKeywords(potion.Desc));

            float curX = position.x + ColumnOffsetX;
            float curTop = position.y + StartOffsetY;
            float columnTop = curTop;

            for (int i = 0; i < allCards.Count; i++)
            {
                (string name, string desc) = allCards[i];
                if (i > 0)
                    desc = mKeywordResolver.FormatDescription(desc.Replace("\\n", "\n"));

                KeywordCard card = mPool.Get(GameRoot.CombatOverlay);
                card.Setup(name, desc);
                mCards.Add(card);

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
        }

        public void Hide()
        {
            foreach (KeywordCard card in mCards)
                mPool.Return(card);
            mCards.Clear();
        }
    }
}
