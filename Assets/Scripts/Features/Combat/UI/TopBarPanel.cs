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
            hero.Health.RegisterWithInitValue(v => LifeLabel.text = $"{v}/{hero.MaxHealth.Value}")
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            hero.MaxHealth.Register(v => LifeLabel.text = $"{hero.Health.Value}/{v}")
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            IResourceModel resource = this.GetModel<IResourceModel>();
            resource.Gold.RegisterWithInitValue(v => CoinLabel.text = v.ToString())
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }
    }
}