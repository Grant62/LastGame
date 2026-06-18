using Features.Combat.Utility;
using Features.Combat.View.Board;
using Features.Enemy.View;
using QFramework;

namespace Main.GM.Command
{
    public class KillAllEnemiesCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            BoardView board = this.GetUtility<IBoardAccess>().Board;
            foreach (EnemyView enemy in board.GetActiveEnemies())
                enemy.ApplyDamage(999999);
        }
    }
}