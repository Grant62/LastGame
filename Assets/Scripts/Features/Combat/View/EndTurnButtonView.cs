using Core.Architecture;
using Features.Combat.Event;
using Features.Combat.Interaction;
using Features.Combat.System;
using QFramework;

namespace Features.Combat.View
{
    public partial class EndTurnButtonView : ViewController, IController
    {
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            this.RegisterEvent<PlayerTurnStartEvent>(OnPlayerTurnStart)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            RefreshText();
        }

        private void OnPlayerTurnStart(PlayerTurnStartEvent e)
        {
            RefreshText();
        }

        private void RefreshText()
        {
            if (Text == null)
                return;

            ITurnSystem turn = this.GetSystem<ITurnSystem>();
            Text.text = $"结束第{turn.TurnCount}回合";
        }

        private void OnMouseDown()
        {
            if (!this.GetSystem<IInteractionSystem>().CanEndTurn())
                return;

            this.GetSystem<ITurnSystem>().EndPlayerTurn();
        }
    }
}