using Features.Card.Event;
using Features.Combat.Event;
using Features.Combat.Targeting;
using Features.Combat.Targeting.System;
using QFramework;

namespace Features.Combat.EffectSystem
{
    public class CardEffectSystem : AbstractSystem, ICardEffectSystem
    {
        private ITargetSelector mTargetSelector;

        protected override void OnInit()
        {
            this.RegisterEvent<CardPlayedEvent>(OnCardPlayed);
            this.RegisterEvent<GameReadyEvent>(_ => OnGameReady());
        }

        private void OnGameReady()
        {
            mTargetSelector = this.GetUtility<ITargetSelector>();
        }

        private void OnCardPlayed(CardPlayedEvent @event)
        {
            ITargetingSystem targeting = this.GetSystem<ITargetingSystem>();
            ITargetable caster = mTargetSelector.GetCaster();

            foreach (Effect effect in @event.CardData.ManualTargetEffect)
            {
                ITargetable target = @event.ManualTarget;
                if (target != null)
                    effect.Execute(new[] { target }, caster);
            }

            foreach (AutoTargetEffect at in @event.CardData.OtherEffects)
            {
                ITargetable[] targets = targeting.ResolveTargets(at.TargetType, caster);
                if (targets.Length > 0)
                    at.Effect.Execute(targets, caster);
            }
        }
    }
}