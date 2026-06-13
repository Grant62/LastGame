using Core.Architecture;
using Features.Combat.Event;
using Features.Combat.System;
using QFramework;
using UnityEngine.UI;

namespace Features.Combat.UI
{
    public partial class EndTurnBtn : ViewController, IController
    {
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            this.RegisterEvent<PlayerTurnStartEvent>(_ => RefreshText())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            Button btn = GetComponent<Button>();
            btn.onClick.AddListener(OnEndTurnClicked);

            RefreshText();
        }

        private void RefreshText()
        {
            ITurnSystem turn = this.GetSystem<ITurnSystem>();
            TurnLabel.text = $"结束第{turn.TurnCount}回合";
        }

        private void OnEndTurnClicked()
        {
            if (!this.GetSystem<IInteractionSystem>().CanEndTurn())
                return;

            this.GetSystem<ITurnSystem>().EndPlayerTurn();
        }
    }
}