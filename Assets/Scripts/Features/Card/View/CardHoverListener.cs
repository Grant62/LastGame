using Core.Architecture;
using Features.Card.Event;
using QFramework;
using UnityEngine;

namespace Features.Card.View
{
    public class CardHoverListener : MonoBehaviour, IController
    {
        [SerializeField] private CardView hoverView;
        [SerializeField] private float verticalOffset = 0.25f;

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
            hoverView.GetComponent<Collider>().enabled = false;
        }

        private void OnCardHover(CardHoverEvent e)
        {
            hoverView.Setup(e.CardData);
            hoverView.transform.position = e.Position + Vector3.up * verticalOffset;
            hoverView.gameObject.SetActive(true);
        }

        private void OnCardHoverEnd()
        {
            hoverView.gameObject.SetActive(false);
        }
    }
}