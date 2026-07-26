using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Shop.View
{
    public class PotionShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private GameObject hoverPanel;
        [SerializeField] private TMP_Text hoverNameText;
        [SerializeField] private TMP_Text hoverDescText;
        [SerializeField] private Button clickButton;

        private Action mOnBuy;

        private void Awake()
        {
            hoverPanel.SetActive(false);
            if (clickButton != null)
                clickButton.onClick.AddListener(() => mOnBuy?.Invoke());
        }

        public void Render(cfg.PotionInfo info, Sprite sprite, bool isSold, Action onBuy)
        {
            mOnBuy = onBuy;
            icon.sprite = sprite;
            priceText.text = $"{info.Price}金";
            hoverNameText.text = info.Name;
            hoverDescText.text = info.Desc;

            if (isSold)
                gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverPanel.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverPanel.SetActive(false);
        }
    }
}