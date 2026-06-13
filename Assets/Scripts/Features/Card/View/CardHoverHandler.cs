using Core.Architecture;
using Features.Card.Interfaces;
using Features.Card.UI;
using Features.Combat.System;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Card.View
{
    public class CardHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IController
    {
        private CardView mCardView;
        private CanvasGroup mCanvasGroup;
        private ICardHoverDisplay mDisplay;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            mCardView = GetComponent<CardView>();
            mCanvasGroup = GetComponentInChildren<CanvasGroup>();
            mDisplay = GetArchitecture().GetUtility<ICardHoverDisplay>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!this.GetSystem<IInteractionSystem>().CanHover())
                return;

            HandPanel panel = GetComponentInParent<HandPanel>();

            mCanvasGroup.alpha = 0f;
            Vector3 hoverPos = transform.position;
            hoverPos.y = panel.HoverCardY;
            mDisplay.Show(mCardView.CardData, hoverPos);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.GetSystem<IInteractionSystem>().IsDragging)
                return;

            mCanvasGroup.alpha = 1f;
            mDisplay.Hide();
        }
    }
}