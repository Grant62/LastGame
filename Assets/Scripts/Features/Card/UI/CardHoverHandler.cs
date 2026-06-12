using Core.Architecture;
using Features.Card.Interfaces;
using Features.Combat.Interaction;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Card.UI
{
    public class CardHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IController
    {
        private CardUI mCardUI;
        private CanvasGroup mCanvasGroup;
        private ICardHoverDisplay mDisplay;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            mCardUI = GetComponent<CardUI>();
            mCanvasGroup = GetComponentInChildren<CanvasGroup>();
            mDisplay = GameMain.Interface.GetUtility<ICardHoverDisplay>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (mCardUI.CardData == null)
                return;
            if (!this.GetSystem<IInteractionSystem>().CanHover())
                return;

            mCanvasGroup.alpha = 0f;
            HandPanel panel = GetComponentInParent<HandPanel>();
            Vector3 hoverPos = transform.position;
            hoverPos.y = panel.HoverCardY;
            mDisplay.Show(mCardUI.CardData, hoverPos);

            IKeywordResolver resolver = GameMain.Interface.GetUtility<IKeywordResolver>();
            string keywords = resolver.ResolveKeywords(mCardUI.CardData.Desc);
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