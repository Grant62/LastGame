using Core.Architecture;
using Features.Combat.Targeting;
using Features.Hero.Command;
using Features.Hero.Model;
using QFramework;
using UnityEngine;

namespace Features.Hero.View
{
    public partial class HeroView : ViewController, IController, IDamageable
    {
        private IHeroModel mHeroModel;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public Vector3 Position { get => transform.position; }

        public bool IsValidTarget { get => mHeroModel != null && mHeroModel.Health.Value > 0; }

        public void TakeDamage(int amount)
        {
            this.SendCommand(new HeroTakeDamageCommand(amount));
        }

        public void TakeHeal(int amount)
        {
            this.SendCommand(new HeroTakeHealCommand(amount));
        }

        private void Start()
        {
            mHeroModel = this.GetModel<IHeroModel>();

            mHeroModel.Health.Register(OnHealthChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            mHeroModel.MaxHealth.Register(OnMaxHealthChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            mHeroModel.Invincible.Register(OnInvincibleChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            RefreshHealthBar();
        }

        private void OnHealthChanged(int health)
        {
            RefreshHealthBar();
        }

        private void OnMaxHealthChanged(int maxHealth)
        {
            RefreshHealthBar();
        }

        private void OnInvincibleChanged(bool invincible) { }

        private void RefreshHealthBar()
        {
            if (Fill == null)
                return;

            float ratio = mHeroModel.MaxHealth.Value > 0
                ? (float)mHeroModel.Health.Value / mHeroModel.MaxHealth.Value
                : 0f;

            Fill.transform.localScale = new Vector3(ratio, 1, 1);

            if (HealthText != null)
            {
                HealthText.text = mHeroModel.Health.Value <= 0
                    ? "死亡"
                    : $"{mHeroModel.Health.Value}/{mHeroModel.MaxHealth.Value}";
            }
        }
    }
}