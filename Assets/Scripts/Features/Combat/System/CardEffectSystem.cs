using Core.Systems;
using Features.Card.Event;
using Features.Card.Model;
using Features.Card.System;
using Features.Combat.Event;
using Features.Combat.Model;
using Features.Combat.Targeting;
using Features.Combat.Targeting.System;
using Features.Combat.Utility;
using Features.Hero.Model;
using Features.Resource.Model;
using Features.Resource.System;
using Features.Sword.Model;
using QFramework;

namespace Features.Combat.System
{
    public class CardEffectSystem : AbstractSystem
    {
        private ITargetSelector mTargetSelector;
        private EffectContext mCtx;

        protected override void OnInit()
        {
            this.RegisterEvent<CardPlayedEvent>(OnCardPlayed);
            this.RegisterEvent<GameReadyEvent>(_ => OnGameReady());
        }

        private void OnGameReady()
        {
            mTargetSelector = this.GetUtility<ITargetSelector>();

            mCtx = new EffectContext(
                this.GetModel<IHeroModel>(),
                this.GetModel<ISwordModel>(),
                this.GetModel<ICardModel>(),
                this.GetSystem<ICardSystem>(),
                this.GetSystem<IResourceSystem>(),
                this.GetModel<IResourceModel>(),
                this.GetUtility<IBoardAccess>(),
                this.GetSystem<IRandomSystem>(),
                this.GetModel<IGameConfigModel>());
        }

        private void OnCardPlayed(CardPlayedEvent @event)
        {
            ITargetingSystem targeting = this.GetSystem<ITargetingSystem>();
            ITargetable caster = mTargetSelector.GetCaster();

            mCtx.EnergySpent = @event.EnergySpent;
            mCtx.SlotTargetIndex = @event.SlotTargetIndex;
            mCtx.PlayedCard = @event.CardData;

            foreach (Effect effect in @event.CardData.ManualTargetEffect)
            {
                effect.Ctx = mCtx;
                ITargetable target = @event.ManualTarget;
                if (target != null)
                    effect.Execute(new[] { target }, caster);
            }

            foreach (AutoTargetEffect atEf in @event.CardData.OtherEffects)
            {
                atEf.Effect.Ctx = mCtx;
                ITargetable[] targets = targeting.ResolveTargets(atEf.TargetType, caster);
                if (targets.Length > 0)
                    atEf.Effect.Execute(targets, caster);
            }

            if (mCtx.CardModel.PendingDiscardCount > 0)
            {
                mCtx.CardModel.PendingDiscardCount = 0;
                this.SendEvent<HandDiscardRequestEvent>();
            }
        }
    }
}