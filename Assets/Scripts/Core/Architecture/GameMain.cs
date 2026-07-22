using Core.Systems;
using Features.Card.Model;
using Features.Card.System;
using Features.Card.Utility;
using Features.Combat.Model;
using Features.Combat.System;
using Features.Enemy.Model;
using Features.Enemy.System;
using Features.Hero.Model;
using Features.Potion.Model;
using Features.Potion.System;
using Features.Resource.Model;
using Features.Resource.System;
using Features.Run.Model;
using Features.Shop.Model;
using Features.Shop.System;
using Features.Sword.Model;
using Features.Sword.System;
using Main.GM;
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
            RegisterSystem<ISwordSystem>(new SwordSystem());
            RegisterModel<IGameConfigModel>(new GameConfigModel());
            RegisterSystem<ISlotTargetSystem>(new SlotTargetSystem());
            RegisterUtility<IBinaryDataMgr>(new BinaryDataMgr());
            RegisterUtility<ICardSpriteCache>(new CardSpriteCache());
            RegisterSystem<IInteractionSystem>(new InteractionSystem());
            RegisterSystem(new StatusTickSystem());
            RegisterSystem(new SpinDamageSystem());

            RegisterModel<IEnemyDefineModel>(new EnemyDefineModel());
            RegisterModel<IEnemyModel>(new EnemyModel());
            RegisterSystem<IEnemyAISystem>(new EnemyAISystem());
            RegisterModel<IRunModel>(new RunModel());
            RegisterSystem<IPopupStackSystem>(new PopupStackSystem());
            RegisterSystem(new GmSystem());
            RegisterModel<IShopModel>(new ShopModel());
            RegisterSystem<IShopSystem>(new ShopSystem());
            RegisterModel<IPotionModel>(new PotionModel());
            RegisterSystem<IPotionSystem>(new PotionSystem());
        }
    }
}