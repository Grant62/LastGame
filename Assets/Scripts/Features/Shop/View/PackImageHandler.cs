using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Shop.View
{
    public class PackImageHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private ShopItemSlot mSlot;

        private void Awake()
        {
            mSlot = GetComponentInParent<ShopItemSlot>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            mSlot.OnClicked();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mSlot.OnHoverEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mSlot.OnHoverExit();
        }
    }
}