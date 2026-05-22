using Core.Systems;
using Features.Card.Model;
using Features.Card.System;
using Features.Combat.EffectSystem;
using Features.Combat.System;
using Features.Combat.Targeting.Model;
using Features.Combat.Targeting.System;
using Features.Hero.Model;
using Features.Hero.System;
using Features.Resource.Model;
using Features.Resource.System;
using QFramework;

namespace Core.Architecture
{
    public class GameMain : Architecture<GameMain>
    {
        protected override void Init()
        {
            RegisterSystem<IRandomSystem>(new RandomSystem());
            RegisterSystem<ITurnSystem>(new TurnSystem());
            RegisterSystem<IHeroSystem>(new HeroSystem());
            RegisterModel<IHeroModel>(new HeroModel());
            RegisterModel<ITargetingModel>(new TargetingModel());
            RegisterSystem<ITargetingSystem>(new TargetingSystem());

            RegisterModel<IResourceModel>(new ResourceModel());
            RegisterSystem<IResourceSystem>(new ResourceSystem());
            RegisterModel<ICardModel>(new CardModel());
            RegisterSystem<ICardSystem>(new CardSystem());
            RegisterSystem<ICardEffectSystem>(new CardEffectSystem());
        }
    }
}