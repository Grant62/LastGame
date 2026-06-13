using Features.Enemy.View;
using QFramework;
using UnityEngine;

namespace Features.Enemy.Command
{
    public class EnemyTakeDamageCommand : AbstractCommand
    {
        private readonly EnemyView mEnemyView;
        private readonly int mAmount;

        public EnemyTakeDamageCommand(EnemyView enemyView, int amount)
        {
            mEnemyView = enemyView;
            mAmount = amount;
        }

        protected override void OnExecute()
        {
            if (mAmount <= 0)
                return;

            mEnemyView.ApplyDamage(mAmount);
        }
    }
}
