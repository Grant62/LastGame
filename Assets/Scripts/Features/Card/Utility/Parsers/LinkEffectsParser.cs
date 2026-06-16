using System.Collections.Generic;
using Features.Card.Effects;
using Features.Combat.Targeting;
using Services;

namespace Features.Card.Utility.Parsers
{
    public static class LinkEffectsParser
    {
        public static void Parse(string desc, List<AutoTargetEffect> auto)
        {
            ParseLinkAll(desc, auto);
            ParseLinkSpiritDamage(desc, auto);
            ParseLinkDebuff(desc, auto);
            ParseLinkEnemyEnergy(desc, auto);
        }

        private static void ParseLinkAll(string desc, List<AutoTargetEffect> auto)
        {
            if (!CardDescriptionParser.ContainsKeyword(desc, "链接"))
                return;

            int customBlock = CardDescriptionParser.ParseBlock(desc);
            if (customBlock <= 0)
                customBlock = desc.Contains("不获得") ? -1 : 0;

            Effect penetration = ParseLinkPenetrationReward(desc);
            auto.Add(new AutoTargetEffect(TargetType.Self, new LinkSwordsEffect(customBlock, penetration)));
        }

        private static Effect ParseLinkPenetrationReward(string desc)
        {
            string[] parts = desc.Split('。');
            foreach (string part in parts)
            {
                if (!part.Contains("穿透"))
                    continue;

                if (CardDescriptionParser.ParseEnergy(part) > 0)
                    return new GainEnergyEffect(CardDescriptionParser.ParseEnergy(part));
                if (CardDescriptionParser.ParseDraw(part) > 0)
                    return new DrawCardsEffect(CardDescriptionParser.ParseDraw(part));
                if (CardDescriptionParser.ParseDamage(part) > 0)
                    return new DealDamageEffect(CardDescriptionParser.ParseDamage(part));
                if (CardDescriptionParser.ParseBlock(part) > 0)
                    return new GainBlockEffect(CardDescriptionParser.ParseBlock(part));
            }

            return null;
        }

        private static void ParseLinkSpiritDamage(string desc, List<AutoTargetEffect> auto)
        {
            if (!CardDescriptionParser.ContainsKeyword(desc, "链接") || !CardDescriptionParser.ContainsKeyword(desc, "灵剑"))
                return;

            int dmg = CardDescriptionParser.ParseDamage(desc);
            if (dmg <= 0) dmg = 6;
            auto.Add(new AutoTargetEffect(TargetType.Self, new LinkSpiritDamageEffect(dmg)));
        }

        private static void ParseLinkDebuff(string desc, List<AutoTargetEffect> auto)
        {
            if (!CardDescriptionParser.ContainsKeyword(desc, "链接") || !desc.Contains("施加"))
                return;

            int stacks = CardDescriptionParser.ParseStatusStacks(desc);
            if (stacks <= 0) stacks = 2;

            if (CardDescriptionParser.ContainsKeyword(desc, "虚弱"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new LinkDebuffEffect(StatusType.Weak, stacks)));

            if (CardDescriptionParser.ContainsKeyword(desc, "易伤"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new LinkDebuffEffect(StatusType.Vulnerable, stacks)));
        }

        private static void ParseLinkEnemyEnergy(string desc, List<AutoTargetEffect> auto)
        {
            if (!CardDescriptionParser.ContainsKeyword(desc, "链接") || !desc.Contains("敌"))
                return;

            int energy = CardDescriptionParser.ParseEnergy(desc);
            if (energy <= 0)
                energy = 1;

            auto.Add(new AutoTargetEffect(TargetType.Self,
                new PerLinkedEnemyEffect(new GainEnergyEffect(energy))));
        }
    }
}