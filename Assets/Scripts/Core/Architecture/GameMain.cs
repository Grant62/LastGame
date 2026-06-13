using Core.Systems;
using Features.Card.Model;
using Features.Card.System;
using Features.Combat.System;
using Features.Combat.Targeting.Model;
using Features.Combat.Targeting.System;
using Features.Hero.Model;
using Features.Resource.Model;
using Features.Resource.System;
using Features.Sword.Model;
using Features.Sword.System;
using QFramework;
using Services.ExcelTool;

namespace Core.Architecture
{
    public class GameMain : Architecture<GameMain>
    {
        protected override void Init()
        {
            RegisterSystem<IRandomSystem>(new RandomSystem());
            RegisterSystem<ITurnSystem>(new TurnSystem());
            RegisterModel<IHeroModel>(new HeroModel());
            RegisterModel<ITargetingModel>(new TargetingModel());
            RegisterSystem<ITargetingSystem>(new TargetingSystem());

            RegisterModel<IResourceModel>(new ResourceModel());
            RegisterSystem<IResourceSystem>(new ResourceSystem());
            RegisterModel<ICardModel>(new CardModel());
            RegisterModel<ICardDefineModel>(new CardDefineModel());
            RegisterSystem<ICardSystem>(new CardSystem());
            RegisterSystem(new CardEffectSystem());
            RegisterModel<ISwordModel>(new SwordModel());
            RegisterSystem<ISlotTargetSystem>(new SlotTargetSystem());
            RegisterUtility<IBinaryDataMgr>(new BinaryDataMgr());
            RegisterSystem<IInteractionSystem>(new InteractionSystem());
            RegisterSystem(new StatusTickSystem());
            RegisterSystem(new SpinDamageSystem());
        }
    }
}