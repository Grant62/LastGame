using Features.Hero.Event;
using Features.Hero.Model;
using QFramework;
using UnityEngine;

namespace Features.Hero.System
{
    public class HeroSystem : AbstractSystem, IHeroSystem
    {
        protected override void OnInit() { }

        public void Setup(HeroDefine define)
        {
            IHeroModel model = this.GetModel<IHeroModel>();
            model.MaxHealth.Value = define.MaxHealth;
            model.Health.Value = define.MaxHealth;
            model.Invincible.Value = define.Invincible;
        }

        public void TakeDamage(int damage)
        {
            IHeroModel model = this.GetModel<IHeroModel>();

            if (damage <= 0)
                return;

            if (model.Invincible.Value)
                return;

            model.Health.Value -= damage;
            model.Health.Value = Mathf.Max(0, model.Health.Value);

            if (model.Health.Value <= 0)
                this.SendEvent<HeroDeathEvent>();
        }

        public void Heal(int amount)
        {
            IHeroModel model = this.GetModel<IHeroModel>();

            if (amount <= 0)
                return;

            model.Health.Value += amount;
            model.Health.Value = Mathf.Min(model.MaxHealth.Value, model.Health.Value);
        }
    }
}