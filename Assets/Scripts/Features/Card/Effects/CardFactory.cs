using System;
using System.Collections.Generic;
using Features.Card.Data;
using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public static class CardFactory
    {
        public static List<CardData> CreateStarterDeck()
        {
            List<CardData> deck = new();
            deck.AddRange(CreateMultiple(CreateSlash, 3));
            deck.AddRange(CreateMultiple(CreateHeavySlash, 2));
            deck.AddRange(CreateMultiple(CreateHeal, 2));
            return deck;
        }

        private static List<CardData> CreateMultiple(Func<CardData> factory, int count)
        {
            List<CardData> cards = new();
            for (int i = 0; i < count; i++)
                cards.Add(factory());
            return cards;
        }

        private static CardData CreateSlash()
        {
            CardData card = new(1, "砍击", 0, "Common", "造成8点伤害", 1, 0, "", 0, 8, 0, 0, "", 0, "");
            card.ManualTargetEffect = new List<Effect> { new DealDamageEffect(8) };
            return card;
        }

        private static CardData CreateHeavySlash()
        {
            CardData card = new(2, "重斩", 0, "Common", "造成15点伤害", 2, 0, "", 0, 15, 0, 0, "", 0, "");
            card.ManualTargetEffect = new List<Effect> { new DealDamageEffect(15) };
            return card;
        }

        private static CardData CreateHeal()
        {
            CardData card = new(3, "治疗", 0, "Common", "回复8点生命", 1, 0, "", 0, 0, 0, 0, "", 0, "");
            card.ManualTargetEffect = new List<Effect> { new DealHealEffect(8) };
            return card;
        }
    }
}