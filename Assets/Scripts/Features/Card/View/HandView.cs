using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Card.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Card.View
{
    public class HandView : MonoBehaviour
    {
        [LabelText("卡牌横向布局/弧形布局")]
        public bool isHorizontal;

        public float maxWidth = 25f;
        public float cardSpacing = 2f;

        [Header("弧形参数")]
        public float maxTotalAngle = 26f;

        public float angleBetweenCards = 4f;
        public float radius = 28f;
        public float centerPointY = -32f;

        [LabelText("实际圆心坐标")]
        public Vector3 centerPoint;

        [SerializeField] private List<Vector3> mCardPositions = new();
        private readonly List<Quaternion> mCardRotations = new();
        private readonly List<CardView> mCardViews = new();

        private const int BaseSortingOrder = 5;
        private const float ZOffset = 0.1f;

        private void Awake()
        {
            centerPoint = isHorizontal ? Vector3.up * -4.5f : Vector3.up * centerPointY;
        }

        public async UniTask AddCardAsync(CardView cardView)
        {
            mCardViews.Add(cardView);
            await SetCardLayoutAsync(0.15f);
        }

        public CardView RemoveCard(CardData card)
        {
            CardView cardView = GetCardView(card);

            if (cardView == null)
                return null;

            mCardViews.Remove(cardView);
            SetCardLayoutAsync(0.15f).Forget();
            return cardView;
        }

        public async UniTask RemoveAllCardsAsync()
        {
            while (mCardViews.Count > 0)
            {
                CardView cardView = mCardViews[0];
                mCardViews.RemoveAt(0);
                Destroy(cardView.gameObject);
                await SetCardLayoutAsync(0.15f);
            }
        }

        public void ForceClearAllCards()
        {
            foreach (CardView cardView in mCardViews)
            {
                if (cardView != null)
                    Destroy(cardView.gameObject);
            }

            mCardViews.Clear();
        }

        public async UniTask SetCardLayoutAsync(float duration, CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < mCardViews.Count; i++)
            {
                CardView curCardView = mCardViews[i];

                if (curCardView == null)
                    continue;

                CardTransform cardTransform = CulAndGetCardTrans(i, mCardViews);
                int sortOrder = BaseSortingOrder + i;
                curCardView.SortingGroup.sortingOrder = sortOrder;
                cardTransform.Pos = new Vector3(cardTransform.Pos.x, cardTransform.Pos.y, -ZOffset * i);
                curCardView.UpdatePositionRotation(cardTransform.Pos, cardTransform.Rot, duration);
            }

            if (duration > 0)
                await UniTask.Delay((int)(duration * 1000), cancellationToken: cancellationToken);
        }

        private CardView GetCardView(CardData card)
        {
            for (int i = 0; i < mCardViews.Count; i++)
            {
                if (mCardViews[i].CardData == card)
                    return mCardViews[i];
            }

            return null;
        }

        public void UpdateCardView(int cardIndex, CardData cardData)
        {
            if (cardIndex < 0 || cardIndex >= mCardViews.Count)
                return;

            mCardViews[cardIndex]?.Setup(cardData);
        }

        private void CalculatePosition(int numberOfCards, bool horizontal)
        {
            mCardPositions.Clear();
            mCardRotations.Clear();

            if (numberOfCards == 0)
                return;

            if (horizontal)
            {
                float currentCardWidth = (numberOfCards - 1) * cardSpacing;
                float totalWidth = Mathf.Min(maxWidth, currentCardWidth);
                float currentSpacing = totalWidth > 0 ? totalWidth / (numberOfCards - 1) : cardSpacing;

                for (int i = 0; i < numberOfCards; i++)
                {
                    float xPos = -(totalWidth / 2) + i * currentSpacing;
                    Vector3 pos = new(xPos, centerPoint.y, 0f);
                    Quaternion rotation = Quaternion.identity;

                    mCardPositions.Add(pos);
                    mCardRotations.Add(rotation);
                }
            }
            else
            {
                float currentTotalAngle = (numberOfCards - 1) * angleBetweenCards;
                float totalAngle = Mathf.Min(maxTotalAngle, currentTotalAngle);
                float currentAngleBetween = totalAngle > 0 ? totalAngle / (numberOfCards - 1) : angleBetweenCards;
                float cardAngle = totalAngle / 2;

                for (int i = 0; i < numberOfCards; i++)
                {
                    float angle = cardAngle - i * currentAngleBetween;
                    Vector3 pos = FanCardPosition(angle);
                    Quaternion rotation = Quaternion.Euler(0, 0, angle);
                    mCardPositions.Add(pos);
                    mCardRotations.Add(rotation);
                }
            }
        }

        private Vector3 FanCardPosition(float angle)
        {
            return new Vector3(
                centerPoint.x - Mathf.Sin(Mathf.Deg2Rad * angle) * radius,
                centerPoint.y + Mathf.Cos(Mathf.Deg2Rad * angle) * radius,
                0
            );
        }

        private CardTransform CulAndGetCardTrans(int cardIndex, List<CardView> cards)
        {
            int numberOfCards = cards.Count;
            CalculatePosition(numberOfCards, isHorizontal);
            return new CardTransform(mCardPositions[cardIndex], mCardRotations[cardIndex]);
        }
    }
}