using Features.Combat.Event;
using Features.Combat.Utility;
using Features.Combat.View.Board;
using Features.Enemy.Model;
using Features.Run.Model;
using QFramework;

namespace Main.GM.Command
{
    public class SkipLevelCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            const int maxLayer = 8;
            IRunModel run = this.GetModel<IRunModel>();

            if (run.CurrentLayer.Value > maxLayer)
            {
                this.SendEvent<BattleVictoryEvent>();
                return;
            }

            BoardView board = this.GetUtility<IBoardAccess>().Board;
            board.ClearAllEnemies();

            IEnemyModel enemyModel = this.GetModel<IEnemyModel>();
            enemyModel.ClearAll();

            run.CurrentStep.Value++;
            if (run.CurrentStep.Value > 3)
            {
                run.CurrentStep.Value = 1;
                run.CurrentLayer.Value++;
            }

            if (run.CurrentLayer.Value > maxLayer)
            {
                this.SendEvent<BattleVictoryEvent>();
                return;
            }

            this.SendEvent(new FloorClearedEvent
            {
                Layer = run.CurrentLayer.Value,
                Step = run.CurrentStep.Value
            });
        }
    }
}