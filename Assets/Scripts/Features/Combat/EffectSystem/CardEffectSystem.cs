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

        private void OnCardPlayed(CardPlayedEvent e)
        {
            ITargetingSystem targeting = this.GetSystem<ITargetingSystem>();

            foreach (Effect effect in e.CardData.ManualTargetEffect)
            {
                ITargetable target = e.ManualTarget;
                if (target != null)
                    effect.Execute(new[] { target }, target);
            }

            foreach (AutoTargetEffect at in e.CardData.OtherEffects)
            {
                ITargetable[] targets = targeting.ResolveTargets(at.TargetType, null);
                if (targets.Length > 0)
                    at.Effect.Execute(targets, null);
            }
        }
    }
}