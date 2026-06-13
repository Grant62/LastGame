using Core.Architecture;
using DG.Tweening;
using Features.Card.Data;
using Features.Resource.Model;
using QFramework;
using UnityEngine;

namespace Features.Card.View
{
    public partial class CardView : ViewController, IController
    {
        private Sprite mDefaultSprite;
        private bool mDefaultCached;
        private IUnRegister mEnergyUnregister;

        public CardData CardData { get; private set; }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void Setup(CardData data)
        {
            if (!mDefaultCached)
            {
                mDefaultSprite = CardImage.sprite;
                mDefaultCached = true;
            }

            CardData = data;
            Title.text = data.Name;
            Desc.text = data.Desc;
            Cost.text = data.Cost == -1 ? "X" : data.Cost.ToString();
            TypeText.text = data.Type;
            Price.text = data.Price.ToString();

            Sprite loaded = LoadIcon(data.IconAddress);
            CardImage.sprite = loaded != null ? loaded : mDefaultSprite;
            OutlineImage.sprite = CardImage.sprite;
            OutlineImage.gameObject.SetActive(false);

            RectTransform rect = GetComponent<RectTransform>();
            rect.DOKill();
            CanvasGroup cg = GetComponentInChildren<CanvasGroup>();
            cg.DOKill();
            cg.alpha = 1f;

            mEnergyUnregister?.UnRegister();
            mEnergyUnregister = this.GetModel<IResourceModel>().CurEnergy
                .Register(_ => UpdateOutline());
            UpdateOutline();
        }

        public void Reset()
        {
            mEnergyUnregister?.UnRegister();
            mEnergyUnregister = null;
            CardData = null;
            CardImage.sprite = mDefaultSprite;
            GetComponent<HandDragHandler>().enabled = true;
        }

        private void UpdateOutline()
        {
            bool canAfford = CardData.Cost == -1
                             || this.GetModel<IResourceModel>().CurEnergy.Value >= CardData.Cost;
            OutlineImage.gameObject.SetActive(canAfford);
        }

        private Sprite LoadIcon(string iconAddress)
        {
            if (string.IsNullOrEmpty(iconAddress))
                return null;

            return null;
        }
    }
}