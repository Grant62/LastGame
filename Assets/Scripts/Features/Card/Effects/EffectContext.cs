using Core.Systems;
using Features.Card.Data;
using Features.Card.Model;
using Features.Card.System;
using Features.Combat.Model;
using Features.Combat.Utility;
using Features.Hero.Model;
using Features.Resource.Model;
using Features.Resource.System;
using Features.Sword.Model;
using Features.Sword.System;

namespace Features.Card.Effects
{
    public class EffectContext
    {
        public IHeroModel HeroModel { get; }
        public ISwordModel SwordModel { get; }
        public ICardModel CardModel { get; }
        public ICardSystem CardSystem { get; }
        public ISwordSystem SwordSystem { get; }
        public IResourceSystem ResourceSystem { get; }
        public IResourceModel ResourceModel { get; }
        public IBoardAccess BoardAccess { get; }
        public IRandomSystem RandomSystem { get; }
        public IGameConfigModel Config { get; }
        public int EnergySpent { get; set; }
        public int SlotTargetIndex { get; set; }
        public CardData PlayedCard { get; set; }

        public EffectContext(
            IHeroModel heroModel,
            ISwordModel swordModel,
            ICardModel cardModel,
            ICardSystem cardSystem,
            ISwordSystem swordSystem,
            IResourceSystem resourceSystem,
            IResourceModel resourceModel,
            IBoardAccess boardAccess,
            IRandomSystem randomSystem,
            IGameConfigModel config)
        {
            HeroModel = heroModel;
            SwordModel = swordModel;
            CardModel = cardModel;
            CardSystem = cardSystem;
            SwordSystem = swordSystem;
            ResourceSystem = resourceSystem;
            ResourceModel = resourceModel;
            BoardAccess = boardAccess;
            RandomSystem = randomSystem;
            Config = config;
        }
    }
}