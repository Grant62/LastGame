using Features.Enemy.Event;
using Features.Enemy.Model;
using QFramework;
using UnityEngine;

namespace Features.Enemy.Command
{
    public class DamageEnemyCommand : AbstractCommand
    {
        private readonly int mDamage;

        public DamageEnemyCommand(int damage)
        {
            mDamage = damage;
        }

        protected override void OnExecute()
        {
            IEnemyModel model = this.GetModel<IEnemyModel>();

            if (mDamage <= 0)
                return;

            model.Health.Value -= mDamage;
            model.Health.Value = Mathf.Max(0, model.Health.Value);

            if (model.Health.Value <= 0)
                this.SendEvent<EnemyDeathEvent>();
        }
    }
}