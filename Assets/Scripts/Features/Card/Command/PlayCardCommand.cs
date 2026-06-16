using Features.Card.Data;
using Features.Card.Event;
using Features.Card.System;
using Features.Combat.Targeting;
using Features.Resource.Model;
using Features.Resource.System;
using QFramework;

namespace Features.Card.Command
{
    public class PlayCardCommand : AbstractCommand
    {
        private readonly CardData mCardData;
        private readonly ITargetable mManualTarget;
        private readonly int mSlotTargetIndex;

        public PlayCardCommand(CardData cardData, ITargetable manualTarget = null, int slotTargetIndex = -1)
        {
            mCardData = cardData;
            mManualTarget = manualTarget;
            mSlotTargetIndex = slotTargetIndex;
        }

        protected override void OnExecute()
        {
            IResourceSystem resource = this.GetSystem<IResourceSystem>();

            int cost = mCardData.Cost == -1
                ? this.GetModel<IResourceModel>().CurEnergy.Value
                : mCardData.Cost;

            if (!resource.CanSpend(cost))
                return;

            resource.Spend(cost);
            this.GetSystem<ICardSystem>().RemoveFromHand(mCardData);
            this.GetSystem<ICardSystem>().AddToDiscard(mCardData);

            this.SendEvent(new CardPlayedEvent(mCardData, mManualTarget, cost, mSlotTargetIndex));
        }
    }
}