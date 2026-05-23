using System;

namespace Features.Card.Command
{
    [Serializable]
    public class CardAmountEntry
    {
        public int cardId;
        public int amount;
    }

    [Serializable]
    public class CardAmountEntryList
    {
        public CardAmountEntry[] entries;
    }
}