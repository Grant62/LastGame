using Core.Architecture;
using DG.Tweening;
using Features.Combat.Event;
using Features.Combat.System;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Combat.UI
{
    public partial class EndTurnBtn : ViewController, IController
    {
        [SerializeField] private Button button;

        private RectTransform mRect;
        private float mOriginY;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            mRect = GetComponent<RectTransform>();
            mOriginY = mRect.anchoredPosition.y;

            this.RegisterEvent<PlayerTurnStartEvent>(_ => OnPlayerTurnStart())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            button.onClick.AddListener(OnEndTurnClicked);
            RefreshText();
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnEndTurnClicked);
        }

        private void RefreshText()
        {
            ITurnSystem turn = this.GetSystem<ITurnSystem>();
            TurnLabel.text = $"结束第{turn.TurnCount}回合";
        }

        private void OnPlayerTurnStart()
        {
            RefreshText();
            mRect.DOAnchorPosY(mOriginY, 0.4f).SetEase(Ease.OutBack);
        }

        private void OnEndTurnClicked()
        {
            if (!this.GetSystem<IInteractionSystem>().CanEndTurn())
                return;

            mRect.DOAnchorPosY(-200f, 0.15f).SetEase(Ease.InBack);
            this.GetSystem<ITurnSystem>().EndPlayerTurn();
        }
    }
}