using System;
using Core.Infrastructure;
using Features.Shop.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Shop.View
{
    public class ShopItemSlot : MonoBehaviour
    {
        [SerializeField] private Image packImage;
        [SerializeField] private GameObject hoverPanel;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descText;
        [SerializeField] private TMP_Text typeText;
        [SerializeField] private TMP_Text priceText;

        private Action mOnBuy;

        public void Render(ShopCardPackSlot data, Sprite sprite, Action onBuy)
        {
            mOnBuy = onBuy;

            nameText.text = GameColors.ColorizeRarity(data.Name);
            descText.text = GameColors.ColorizeRarity(data.Desc.Replace("\\n", "\n"));
            typeText.text = GameColors.ColorizeRarity(data.RarityFilter);
            priceText.text = $"{data.Price}金";
            hoverPanel.SetActive(false);

            packImage.sprite = sprite;

            if (data.IsSold)
                Destroy(gameObject);
        }

        public void OnClicked()
        {
            mOnBuy.Invoke();
        }

        public void OnHoverEnter()
        {
            hoverPanel.SetActive(true);
        }

        public void OnHoverExit()
        {
            hoverPanel.SetActive(false);
        }
    }
}