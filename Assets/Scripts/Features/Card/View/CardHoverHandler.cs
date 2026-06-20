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
        private IHoverContext mHoverContext;
        private HandPanel mHandPanel;
        private int mHandIndex = -1;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            mCardView = GetComponent<CardView>();
            mCanvasGroup = GetComponentInChildren<CanvasGroup>();
            mDisplay = this.GetUtility<ICardHoverDisplay>();
            mHoverContext = GetComponentInParent<HandPanel>();
            mHandPanel = mHoverContext as HandPanel;
        }

        public void RegisterHandPanel(HandPanel panel, int index)
        {
            mHandPanel = panel;
            mHandIndex = index;
        }

        public void SetHandIndex(int index)
        {
            mHandIndex = index;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!this.GetSystem<IInteractionSystem>().CanHover())
                return;

            if (mHoverContext == null)
                return;

            if (mHandPanel != null && mHandIndex >= 0)
                mHandPanel.OnCardHovered(mHandIndex);

            mCanvasGroup.alpha = 0f;
            Vector3 hoverPos = transform.position;
            hoverPos.y = mHoverContext.HoverCardY;
            mDisplay.Show(mCardView.CardData, hoverPos);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.GetSystem<IInteractionSystem>().IsDragging)
                return;

            if (mHandPanel != null && mHandIndex >= 0)
                mHandPanel.OnCardUnhovered(mHandIndex);

            mCanvasGroup.alpha = 1f;
            mDisplay.Hide();
        }

        private void OnDestroy()
        {
            if (mHandPanel != null && mHandIndex >= 0)
                mHandPanel.OnCardUnhovered(mHandIndex);
        }
    }
}