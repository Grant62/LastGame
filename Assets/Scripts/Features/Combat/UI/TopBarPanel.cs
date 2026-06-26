using Core.Architecture;
using Features.Hero.Model;
using Features.Resource.Model;
using QFramework;

namespace Features.Combat.UI
{
    public partial class TopBarPanel : ViewController, IController
    {
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            IHeroModel hero = this.GetModel<IHeroModel>();

            hero.Health.RegisterWithInitValue(v => RefreshLifeLabel(hero))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            hero.MaxHealth.Register(_ => RefreshLifeLabel(hero))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            hero.Invincible.Register(_ => RefreshLifeLabel(hero))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            IResourceModel resource = this.GetModel<IResourceModel>();
            resource.Gold.RegisterWithInitValue(v => CoinLabel.text = v.ToString())
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void RefreshLifeLabel(IHeroModel hero)
        {
            LifeLabel.text = hero.Invincible.Value
                ? "\u221e"
                : $"{hero.Health.Value}/{hero.MaxHealth.Value}";
        }
    }
}