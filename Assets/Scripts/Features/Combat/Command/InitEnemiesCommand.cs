using Features.Combat.Utility;
using Features.Combat.View.Board;
using Features.Enemy.View;
using QFramework;

namespace Features.Combat.Command
{
    public class InitEnemiesCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            BoardView board = this.GetUtility<IBoardAccess>().Board;

            const int centerSlot = 4;
            int[] hpValues = { 40, 50, 60, 70, 80 };
            int[] dmgValues = { 5, 5, 6, 6, 7 };
            int totalSlots = 9;
            int sideCount = totalSlots / 2;

            int[] spawnOrder = new int[sideCount * 2];
            for (int i = 0; i < sideCount; i++)
            {
                spawnOrder[i * 2] = centerSlot - (i + 1);
                spawnOrder[i * 2 + 1] = centerSlot + i + 1;
            }

            for (int i = 0; i < hpValues.Length; i++)
            {
                int slotIndex = spawnOrder[i];
                EnemyView enemy = board.SpawnEnemy(slotIndex);
                enemy.Init(1000 + i, hpValues[i], dmgValues[i]);
            }
        }
    }
}