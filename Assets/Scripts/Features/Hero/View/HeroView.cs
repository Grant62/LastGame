using Core.Architecture;
using DG.Tweening;
using Features.Combat.Targeting;
using Features.Hero.Command;
using Features.Hero.Model;
using QFramework;
using UnityEngine;

namespace Features.Hero.View
{
    public partial class HeroView : ViewController, IController, IDamageable
    {
        [SerializeField] private Transform characterRoot;

        private IHeroModel mHeroModel;
        private Tween mHealthTween;

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
            mHeroModel.IsFacingRight.Register(OnFacingChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            RefreshHealthBar();
            OnFacingChanged(mHeroModel.IsFacingRight.Value);
        }

        private void OnHealthChanged(int health)
        {
            RefreshHealthBar(true);
        }

        private void OnMaxHealthChanged(int maxHealth)
        {
            RefreshHealthBar(true);
        }

        private void OnFacingChanged(bool facingRight)
        {
            if (characterRoot != null)
            {
                Vector3 scale = characterRoot.localScale;
                scale.x = Mathf.Abs(scale.x) * (facingRight ? 1 : -1);
                characterRoot.localScale = scale;
            }
        }

        private void RefreshHealthBar(bool animate = false)
        {
            mHealthTween?.Kill();
            float ratio = mHeroModel.MaxHealth.Value > 0
                ? (float)mHeroModel.Health.Value / mHeroModel.MaxHealth.Value
                : 0f;

            if (animate)
                mHealthTween = Fill.transform.DOScaleX(ratio, 0.3f);
            else
                Fill.transform.localScale = new Vector3(ratio, 1, 1);

            HealthText.text = mHeroModel.Health.Value <= 0
                ? "死亡"
                : $"{mHeroModel.Health.Value}/{mHeroModel.MaxHealth.Value}";
        }
    }
}