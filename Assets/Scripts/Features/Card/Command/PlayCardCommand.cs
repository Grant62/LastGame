using Features.Card.Data;
using Features.Card.Event;
using Features.Card.System;
using Features.Combat.Targeting;
using Features.Resource.System;
using QFramework;

namespace Features.Card.Command
{
    public class PlayCardCommand : AbstractCommand
    {
        private readonly CardData mCardData;
        private readonly ITargetable mManualTarget;

        public PlayCardCommand(CardData cardData, ITargetable manualTarget = null)
        {
            mCardData = cardData;
            mManualTarget = manualTarget;
        }

        protected override void OnExecute()
        {
            IResourceSystem resource = this.GetSystem<IResourceSystem>();

            if (!resource.CanSpend(mCardData.Cost))
                return;

            resource.Spend(mCardData.Cost);
            this.GetSystem<ICardSystem>().RemoveFromHand(mCardData);

            this.SendEvent(new CardPlayedEvent(mCardData, mManualTarget));
        }
    }
}