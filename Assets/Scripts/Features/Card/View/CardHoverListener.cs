using Core.Architecture;
using Features.Card.Event;
using QFramework;
using UnityEngine;

namespace Features.Card.View
{
    public class CardHoverListener : MonoBehaviour, IController
    {
        [SerializeField] private CardView mHoverView;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            this.RegisterEvent<CardHoverEvent>(OnCardHover)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<CardHoverEndEvent>(_ => OnCardHoverEnd())
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            mHoverView.GetComponent<Collider>().enabled = false;
        }

        private void OnCardHover(CardHoverEvent e)
        {
            mHoverView.Setup(e.CardData);
            mHoverView.transform.position = e.Position + Vector3.up * 0.5f;
            mHoverView.gameObject.SetActive(true);
        }

        private void OnCardHoverEnd()
        {
            mHoverView.gameObject.SetActive(false);
        }
    }
}