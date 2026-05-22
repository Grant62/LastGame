using Core.Architecture;
using Features.Card.Event;
using QFramework;
using UnityEngine;

namespace Features.Card.View
{
    public class CardClickHandler : MonoBehaviour, IController, ICanSendEvent
    {
        private CardView mCardView;
        private bool mIsTweening;

        private CardView CardView { get => mCardView ??= GetComponent<CardView>(); }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void SetTweening(bool isTweening)
        {
            mIsTweening = isTweening;
        }

        private void OnMouseDown()
        {
            if (mIsTweening)
                return;

            this.SendEvent(new CardSelectedEvent(CardView.CardData));
        }

        private void OnMouseEnter()
        {
            if (mIsTweening)
                return;

            Vector3 pos = new(CardView.transform.position.x, -2.8f, 0);
            this.SendEvent(new CardHoverEvent(CardView.CardData, pos));
        }

        private void OnMouseExit()
        {
            this.SendEvent(new CardHoverEndEvent());
        }
    }
}