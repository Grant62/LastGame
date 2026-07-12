using Features.Card.Data;
using Features.Card.System;
using QFramework;

namespace Features.Card.Command
{
    public class AddCardToLibraryCommand : AbstractCommand
    {
        private readonly CardData mCard;

        public AddCardToLibraryCommand(CardData card)
        {
            mCard = card;
        }

        protected override void OnExecute()
        {
            this.GetSystem<ICardSystem>().AddToLibrary(mCard);
        }
    }
}