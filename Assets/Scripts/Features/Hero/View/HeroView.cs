using Core.Architecture;
using Features.Hero.Model;
using QFramework;
using UnityEngine;

namespace Features.Hero.View
{
    public partial class HeroView : ViewController, IController
    {
        private IHeroModel mHeroModel;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            mHeroModel = this.GetModel<IHeroModel>();

            mHeroModel.Health.Register(OnHealthChanged);
            mHeroModel.MaxHealth.Register(OnMaxHealthChanged);
            mHeroModel.Invincible.Register(OnInvincibleChanged);

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

            float ratio = (float)mHeroModel.Health.Value / mHeroModel.MaxHealth.Value;
            Fill.transform.localScale = new Vector3(ratio, 1, 1);
        }
    }
}