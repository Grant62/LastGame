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
            mDisplay = GameMain.Interface.GetUtility<ICardHoverDisplay>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (mCardView.CardData == null)
                return;
            if (!this.GetSystem<IInteractionSystem>().CanHover())
                return;

            mCanvasGroup.alpha = 0f;
            HandPanel panel = GetComponentInParent<HandPanel>();
            Vector3 hoverPos = transform.position;
            hoverPos.y = panel.HoverCardY;
            mDisplay.Show(mCardView.CardData, hoverPos);

            IKeywordResolver resolver = GameMain.Interface.GetUtility<IKeywordResolver>();
            string keywords = resolver.ResolveKeywords(mCardView.CardData.Desc);
            if (!string.IsNullOrEmpty(keywords))
                Debug.Log(keywords);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mCanvasGroup.alpha = 1f;
            mDisplay.Hide();
        }
    }
}