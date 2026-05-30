using Features.Card.Event;
using Features.Combat.Targeting;
using Features.Combat.Targeting.System;
using QFramework;

namespace Features.Combat.EffectSystem
{
    public class CardEffectSystem : AbstractSystem, ICardEffectSystem
    {
        protected override void OnInit()
        {
            this.RegisterEvent<CardPlayedEvent>(OnCardPlayed);
        }

        private void OnCardPlayed(CardPlayedEvent @event)
        {
            ITargetingSystem targeting = this.GetSystem<ITargetingSystem>();
            ITargetable caster = this.GetUtility<ITargetSelector>().GetCaster();

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