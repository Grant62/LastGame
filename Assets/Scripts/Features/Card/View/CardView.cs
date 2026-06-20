using Core.Architecture;
using DG.Tweening;
using Features.Card.Data;
using Features.Card.Utility;
using Features.Resource.Model;
using QFramework;
using UnityEngine;

namespace Features.Card.View
{
    public partial class CardView : ViewController, IController
    {
        [SerializeField] private HandDragHandler handDragHandler;
        [SerializeField] private CardHoverHandler cardHoverHandler;
        [SerializeField] private CanvasGroup canvasGroup;

        private IUnRegister mEnergyUnregister;

        public CardData CardData { get; private set; }

        public HandDragHandler HandDragHandler { get => handDragHandler; }

        public CardHoverHandler CardHoverHandler { get => cardHoverHandler; }

        public CanvasGroup CanvasGroup { get => canvasGroup; }

        public RectTransform RectTransform { get; private set; }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public void Setup(CardData data, bool enableEffects = true)
        {
            CardData = data;
            Title.text = data.Name;
            Desc.text = data.Desc.Replace("\\n", "\n").Replace("【", "").Replace("】", "");
            Cost.text = data.Cost == -1 ? "X" : data.Cost.ToString();
            TypeText.text = data.Type;
            Price.text = data.Price.ToString();

            Sprite icon = this.GetUtility<ICardSpriteCache>().GetSprite(data.IconAddress);
            CardImage.sprite = icon;
            OutlineImage.sprite = icon;
            OutlineImage.gameObject.SetActive(false);

            RectTransform.DOKill();
            canvasGroup.DOKill();
            canvasGroup.alpha = 1f;

            if (enableEffects)
            {
                mEnergyUnregister?.UnRegister();
                mEnergyUnregister = this.GetModel<IResourceModel>().CurEnergy
                    .Register(_ => UpdateOutline());
                UpdateOutline();
            }
        }

        public void Reset()
        {
            mEnergyUnregister?.UnRegister();
            mEnergyUnregister = null;
            CardData = null;
            handDragHandler.enabled = true;
            cardHoverHandler.enabled = true;

            RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            RectTransform.pivot = new Vector2(0.5f, 0.5f);
            RectTransform.localScale = Vector3.one;
            RectTransform.localEulerAngles = Vector3.zero;
            CanvasGroup.alpha = 1f;
        }

        public void RefreshCost()
        {
            Cost.text = CardData.Cost == -1 ? "X" : CardData.Cost.ToString();
            UpdateOutline();
        }

        private void UpdateOutline()
        {
            bool canAfford = CardData.Cost == -1
                             || this.GetModel<IResourceModel>().CurEnergy.Value >= CardData.Cost;
            OutlineImage.gameObject.SetActive(canAfford);
        }
    }
}