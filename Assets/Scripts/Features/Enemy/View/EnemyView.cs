using Core.Architecture;
using Features.Combat.Targeting;
using Features.Enemy.Command;
using Features.Enemy.Model;
using QFramework;
using UnityEngine;

namespace Features.Enemy.View
{
    public partial class EnemyView : ViewController, IController, IDamageable
    {
        private IEnemyModel mEnemyModel;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public Vector3 Position { get => transform.position; }

        public bool IsValidTarget { get => mEnemyModel != null && mEnemyModel.Health.Value > 0; }

        public void TakeDamage(int amount)
        {
            this.SendCommand(new DamageEnemyCommand(amount));
        }

        public void TakeHeal(int amount) { }

        private void Start()
        {
            mEnemyModel = this.GetModel<IEnemyModel>();

            mEnemyModel.Health.Register(OnHealthChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            mEnemyModel.MaxHealth.Register(OnMaxHealthChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            RefreshHealthBar();
        }

        public void Init(int monsterId, int maxHealth, int damage)
        {
            this.SendCommand(new SetupEnemyCommand(new EnemyDefine
            {
                MonsterId = monsterId,
                MaxHealth = maxHealth,
                Damage = damage
            }));
        }

        private void OnHealthChanged(int health)
        {
            RefreshHealthBar();
        }

        private void OnMaxHealthChanged(int maxHealth)
        {
            RefreshHealthBar();
        }

        private void RefreshHealthBar()
        {
            if (Fill == null)
                return;

            float ratio = mEnemyModel.MaxHealth.Value > 0
                ? (float)mEnemyModel.Health.Value / mEnemyModel.MaxHealth.Value
                : 0f;

            Fill.transform.localScale = new Vector3(ratio, 1, 1);
        }
    }
}