using Features.Enemy.View;
using QFramework;

namespace Features.Enemy.Command
{
    public class EnemyGainArmorCommand : AbstractCommand
    {
        private readonly EnemyView mEnemyView;
        private readonly int mAmount;

        public EnemyGainArmorCommand(EnemyView enemyView, int amount)
        {
            mEnemyView = enemyView;
            mAmount = amount;
        }

        protected override void OnExecute()
        {
            if (mAmount <= 0)
                return;

            mEnemyView.ApplyArmor(mAmount);
        }
    }
}
