using Core.Architecture;
using Features.Card.Data;
using QFramework;
using UnityEngine;

namespace Features.Card.UI
{
    public partial class CardUI : ViewController, IController
    {
        private Sprite mDefaultSprite;
        private bool mDefaultCached;

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
            Cost.text = data.Cost.ToString();
            TypeText.text = data.Type;
            Price.text = data.Price.ToString();

            Sprite loaded = LoadIcon(data.IconAddress);
            CardImage.sprite = loaded != null ? loaded : mDefaultSprite;
        }

        public void Reset()
        {
            CardData = null;
            CardImage.sprite = mDefaultSprite;
        }

        private Sprite LoadIcon(string iconAddress)
        {
            if (string.IsNullOrEmpty(iconAddress))
                return null;

            // Placeholder: attempt to load sprite from Resources or ResKit
            return null;
        }
    }
}