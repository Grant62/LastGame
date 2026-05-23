using Cysharp.Threading.Tasks;
using DG.Tweening;
using Features.Card.Data;
using UnityEngine;
using UnityEngine.Rendering;

namespace Features.Card.View
{
    public partial class CardView : MonoBehaviour
    {
        public CardData CardData { get; private set; }

        private SortingGroup mSortingGroup;

        public SortingGroup SortingGroup { get => mSortingGroup ??= GetComponent<SortingGroup>(); }

        public void Setup(CardData data)
        {
            SetCardData(data);
            SetTitle(data.Name);
            SetDescription(data.Desc);
            SetCost(data.Cost.ToString());
            SetType(data.Rarity);
            LoadImageAsync(data).Forget();
        }

        private async UniTaskVoid LoadImageAsync(CardData data)
        {
            if (string.IsNullOrEmpty(data.IconAddress))
                return;

            ResourceRequest request = Resources.LoadAsync<Sprite>(data.IconAddress);
            await request.ToUniTask();

            if (request.asset is Sprite sprite)
                CardImage.sprite = sprite;
        }

        public void SetCardData(CardData data)
        {
            CardData = data;
        }

        public void SetTitle(string titleText)
        {
            Title.text = titleText;
        }

        public void SetDescription(string description)
        {
            DescText.text = description;
        }

        public void SetCost(string costText)
        {
            Cost.text = costText;
        }

        public void SetType(string typeText)
        {
            TypeText.text = typeText;
        }

        public void SetRarityColor(Color color)
        {
            Title.color = color;
        }

        public void SetImage(Sprite sprite)
        {
            CardImage.sprite = sprite;
        }

        public void UpdatePositionRotation(Vector3 pos, Quaternion rot, float duration)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMove(pos, duration));
            seq.Join(transform.DORotate(rot.eulerAngles, duration));
        }
    }
}