using Core.Architecture;
using Features.Hero.Model;
using Features.Resource.Model;
using Features.Run.Model;
using QFramework;
using TMPro;
using UnityEngine;

namespace Features.Combat.UI
{
    public partial class TopBarPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelLabel;

        private void Start()
        {
            IHeroModel hero = GameMain.Interface.GetModel<IHeroModel>();

            hero.Health.RegisterWithInitValue(v => RefreshLifeLabel(hero))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            hero.MaxHealth.Register(_ => RefreshLifeLabel(hero))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            hero.Invincible.Register(_ => RefreshLifeLabel(hero))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            IResourceModel resource = GameMain.Interface.GetModel<IResourceModel>();
            resource.Gold.RegisterWithInitValue(v => CoinLabel.text = v.ToString())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            IRunModel run = GameMain.Interface.GetModel<IRunModel>();
            run.CurrentLayer.RegisterWithInitValue(_ => RefreshLevelLabel(run))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            run.CurrentStep.RegisterWithInitValue(_ => RefreshLevelLabel(run))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void RefreshLevelLabel(IRunModel run)
        {
            levelLabel.text = $"关卡 {run.CurrentLayer.Value}-{run.CurrentStep.Value}";
        }

        private void RefreshLifeLabel(IHeroModel hero)
        {
            LifeLabel.text = hero.Invincible.Value
                ? "\u221e"
                : $"{hero.Health.Value}/{hero.MaxHealth.Value}";
        }
    }
}