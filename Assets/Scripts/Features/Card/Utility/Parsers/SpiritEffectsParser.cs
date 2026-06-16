using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Effects;
using Features.Combat.Targeting;
using Services;

namespace Features.Card.Utility.Parsers
{
    public static class SpiritEffectsParser
    {
        public static void Parse(string desc, List<AutoTargetEffect> auto, CardData cardData)
        {
            ParseHealIfSpirit(desc, auto);
            ParseConditionalSpawnSpirit(desc, auto);
            ParsePerSpiritReward(desc, auto, cardData);
            ParseSpiritAttach(desc, auto);
            ParseSpawnSpiritsRandom(desc, auto);
            ParseTurnStartSpiritSpawn(desc, auto);
            ParseReactiveSpiritSpawn(desc, auto);
        }

        private static void ParseSpiritAttach(string desc, List<AutoTargetEffect> auto)
        {
            if (CardDescriptionParser.ContainsKeyword(desc, "附着·灵")
                || desc.Contains("附着·灵"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.IsSpiritAttached)));
        }

        private static void ParseSpawnSpiritsRandom(string desc, List<AutoTargetEffect> auto)
        {
            if (desc.Contains("随机") && desc.Contains("空位置") && CardDescriptionParser.ContainsAnyKeyword(desc, "灵剑", "生成"))
            {
                int count = 1;
                if (desc.Contains("3处")) count = 3;

                auto.Add(new AutoTargetEffect(TargetType.Self, new SpawnSpiritsAtRandomEffect(count)));
            }
        }

        private static void ParseTurnStartSpiritSpawn(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("每回合开始") || !CardDescriptionParser.ContainsKeyword(desc, "灵剑"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.HasTurnStartSpiritSpawn)));
        }

        private static void ParseReactiveSpiritSpawn(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("每摧毁") || !desc.Contains("空位置"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.HasReactiveSpiritSpawn)));
        }

        private static void ParsePerSpiritReward(string desc, List<AutoTargetEffect> auto, CardData cardData)
        {
            if (!desc.Contains("每") || !desc.Contains("灵剑"))
                return;

            Effect reward = null;
            if (CardDescriptionParser.ParseEnergy(desc) > 0)
                reward = new GainEnergyEffect(CardDescriptionParser.ParseEnergy(desc));
            else if (CardDescriptionParser.ParseDraw(desc) > 0)
                reward = new DrawCardsEffect(CardDescriptionParser.ParseDraw(desc));
            else if (CardDescriptionParser.ParseDamage(desc) > 0)
                reward = new DealDamageEffect(CardDescriptionParser.ParseDamage(desc));

            if (reward != null)
                auto.Add(new AutoTargetEffect(TargetType.Self, new PerSpiritSwordEffect(reward)));
        }

        private static void ParseHealIfSpirit(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("附着") || !desc.Contains("恢复"))
                return;

            int heal = CardDescriptionParser.ParseHeal(desc);
            if (heal <= 0) heal = CardDescriptionParser.ParseBlock(desc);
            if (heal > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self,
                    new ConditionalEffect(new SpiritAttachedCondition(), new HealEffect(heal))));
        }

        private static void ParseConditionalSpawnSpirit(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("附着") || !desc.Contains("指定位置") || !desc.Contains("灵剑"))
                return;

            if (desc.Contains("销毁") || desc.Contains("摧毁"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self,
                new ConditionalEffect(new SpiritAttachedCondition(), new SpawnSpiritAtSlotEffect())));
        }
    }
}