using Core.Systems;
using Features.Card.Model;
using Features.Card.System;
using Features.Combat.Utility;
using Features.Hero.Model;
using Features.Resource.System;
using Features.Sword.Model;

namespace Features.Combat.Targeting
{
    public class EffectContext
    {
        public IHeroModel HeroModel { get; }
        public ISwordModel SwordModel { get; }
        public ICardModel CardModel { get; }
        public ICardSystem CardSystem { get; }
        public IResourceSystem ResourceSystem { get; }
        public IBoardAccess BoardAccess { get; }
        public IRandomSystem RandomSystem { get; }

        public EffectContext(
            IHeroModel heroModel,
            ISwordModel swordModel,
            ICardModel cardModel,
            ICardSystem cardSystem,
            IResourceSystem resourceSystem,
            IBoardAccess boardAccess,
            IRandomSystem randomSystem)
        {
            HeroModel = heroModel;
            SwordModel = swordModel;
            CardModel = cardModel;
            CardSystem = cardSystem;
            ResourceSystem = resourceSystem;
            BoardAccess = boardAccess;
            RandomSystem = randomSystem;
        }
    }
}