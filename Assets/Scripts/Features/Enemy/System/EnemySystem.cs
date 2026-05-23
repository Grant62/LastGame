using Features.Enemy.Event;
using Features.Enemy.Model;
using QFramework;
using UnityEngine;

namespace Features.Enemy.System
{
    public class EnemySystem : AbstractSystem, IEnemySystem
    {
        protected override void OnInit() { }

        public void Setup(EnemyDefine define)
        {
            IEnemyModel model = this.GetModel<IEnemyModel>();
            model.MonsterId.Value = define.MonsterId;
            model.MaxHealth.Value = define.MaxHealth;
            model.Health.Value = define.MaxHealth;
            model.Damage.Value = define.Damage;
        }

        public void TakeDamage(int damage)
        {
            IEnemyModel model = this.GetModel<IEnemyModel>();

            if (damage <= 0)
                return;

            model.Health.Value -= damage;
            model.Health.Value = Mathf.Max(0, model.Health.Value);

            if (model.Health.Value <= 0)
                this.SendEvent<EnemyDeathEvent>();
        }
    }
}