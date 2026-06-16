using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Effects;
using Features.Combat.Targeting;
using Services;

namespace Features.Card.Utility.Parsers
{
    public static class KeywordParser
    {
        public static void Parse(string desc, CardData cardData, List<AutoTargetEffect> auto)
        {
            ParseKeywordRecall(desc, auto);
            ParseKeywordExhaust(desc, cardData, auto);
            ParseKeywordDestroySpirit(desc, auto);
        }

        private static void ParseKeywordRecall(string desc, List<AutoTargetEffect> auto)
        {
            if (!CardDescriptionParser.ContainsKeyword(desc, "剑来"))
                return;

            if (desc.Contains("如果摧毁灵剑"))
            {
                int draw = CardDescriptionParser.ParseDraw(desc);
                if (draw > 0)
                    auto.Add(new AutoTargetEffect(TargetType.Self,
                        new ConditionalEffect(new HasSpiritSwordCondition(),
                            new DrawCardsEffect(draw))));
            }

            auto.Add(new AutoTargetEffect(TargetType.Self, new RecallSwordsEffect()));
        }

        private static void ParseKeywordExhaust(string desc, CardData cardData, List<AutoTargetEffect> auto)
        {
            if (CardDescriptionParser.ContainsKeyword(desc, "消耗"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new ExhaustEffect(cardData)));
        }

        private static void ParseKeywordDestroySpirit(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("摧毁") || !desc.Contains("灵剑") || desc.Contains("生成灵剑"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self, new DestroySpiritEffect()));

            if (desc.Contains("恢复能量至上限"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new RestoreEnergyToMaxEffect()));
        }
    }
}