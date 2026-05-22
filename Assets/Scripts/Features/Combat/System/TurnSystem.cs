using Cysharp.Threading.Tasks;
using Features.Combat.Event;
using QFramework;

namespace Features.Combat.System
{
    public class TurnSystem : AbstractSystem, ITurnSystem
    {
        public int TurnCount { get; private set; }
        public bool IsPlayerTurn { get; private set; }

        protected override void OnInit() { }

        public void StartBattle()
        {
            TurnCount = 0;
            this.SendEvent<BattleStartEvent>();
            StartPlayerTurn();
        }

        public void EndPlayerTurn()
        {
            if (!IsPlayerTurn)
                return;

            IsPlayerTurn = false;
            this.SendEvent<PlayerTurnEndEvent>();
            StartEnemyTurn().Forget();
        }

        private void StartPlayerTurn()
        {
            TurnCount++;
            IsPlayerTurn = true;
            this.SendEvent<PlayerTurnStartEvent>();
        }

        private async UniTaskVoid StartEnemyTurn()
        {
            this.SendEvent<EnemyTurnStartEvent>();

            await UniTask.Delay(1000);

            this.SendEvent<EnemyTurnEndEvent>();
            StartPlayerTurn();
        }
    }
}