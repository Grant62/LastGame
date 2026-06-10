using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Define;
using Features.Card.Effects;
using Features.Combat.Targeting;
using Services;

namespace Features.Card
{
    public static class CardEffectFactory
    {
        public static void PopulateEffects(CardDefine define, CardData cardData)
        {
            string desc = define.Desc;
            List<Effect> manual = new();
            List<AutoTargetEffect> auto = new();

            ParseDamage(desc, define.NeedsEnemyTarget, manual, auto);
            ParseBlock(desc, auto);
            ParseDraw(desc, auto);
            ParseEnergy(desc, auto);
            ParseDiscard(desc, auto);
            ParseHeal(desc, auto);

            ParseKeywordRecall(desc, auto);
            ParseKeywordLink(desc, auto);
            ParseKeywordExhaust(desc, cardData, auto);

            ParseSpin(desc, auto);
            ParseStatus(desc, define.NeedsEnemyTarget, manual, auto);

            cardData.ManualTargetEffect = manual;
            cardData.OtherEffects = auto;
        }

        private static void ParseDamage(string desc, bool needsEnemy, List<Effect> manual, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseDamage(desc);
            if (v <= 0) return;

            if (needsEnemy)
                manual.Add(new DealDamageEffect(v));
            else
                auto.Add(new AutoTargetEffect(TargetType.AllEnemies, new DealDamageEffect(v)));
        }

        private static void ParseBlock(string desc, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseBlock(desc);
            if (v > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self, new GainBlockEffect(v)));
        }

        private static void ParseDraw(string desc, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseDraw(desc);
            if (v > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self, new DrawCardsEffect(v)));
        }

        private static void ParseEnergy(string desc, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseEnergy(desc);
            if (v > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self, new GainEnergyEffect(v)));
        }

        private static void ParseDiscard(string desc, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseDiscard(desc);
            if (v > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self, new DiscardCardsEffect(v)));
        }

        private static void ParseHeal(string desc, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseHeal(desc);
            if (v > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self, new DealHealEffect(v)));
        }

        private static void ParseKeywordRecall(string desc, List<AutoTargetEffect> auto)
        {
            if (CardDescriptionParser.ContainsKeyword(desc, "剑来"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new RecallSwordsEffect()));
        }

        private static void ParseKeywordLink(string desc, List<AutoTargetEffect> auto)
        {
            if (CardDescriptionParser.ContainsKeyword(desc, "链接"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new LinkSwordsEffect()));
        }

        private static void ParseKeywordExhaust(string desc, CardData cardData, List<AutoTargetEffect> auto)
        {
            if (CardDescriptionParser.ContainsKeyword(desc, "消耗"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new ExhaustEffect(cardData)));
        }

        private static void ParseSpin(string desc, List<AutoTargetEffect> auto)
        {
            int count = CardDescriptionParser.ParseSpinCount(desc);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                    auto.Add(new AutoTargetEffect(TargetType.Self, new SpinSwordEffect()));
                return;
            }

            if (CardDescriptionParser.ContainsKeyword(desc, "旋剑"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new SpinSwordEffect()));
        }

        private static void ParseStatus(string desc, bool needsEnemy, List<Effect> manual, List<AutoTargetEffect> auto)
        {
            int stacks = CardDescriptionParser.ParseStatusStacks(desc);
            if (stacks <= 0) return;

            if (CardDescriptionParser.ContainsKeyword(desc, "虚弱"))
            {
                if (needsEnemy)
                    manual.Add(new ApplyWeakEffect(stacks));
                else
                    auto.Add(new AutoTargetEffect(TargetType.RandomEnemy, new ApplyWeakEffect(stacks)));
            }

            if (CardDescriptionParser.ContainsKeyword(desc, "易伤"))
            {
                if (needsEnemy)
                    manual.Add(new ApplyVulnerableEffect(stacks));
                else
                    auto.Add(new AutoTargetEffect(TargetType.RandomEnemy, new ApplyVulnerableEffect(stacks)));
            }
        }
    }
}