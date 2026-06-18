using System.Collections.Generic;
using Features.Card.Effects;
using Features.Combat.Targeting;
using Services;

namespace Features.Card.Utility.Parsers
{
    public static class BasicEffectsParser
    {
        public static void Parse(string desc, bool needsEnemy, List<Effect> manual, List<AutoTargetEffect> auto)
        {
            string unconditional = UnconditionalPart(desc);

            ParseDamage(unconditional, needsEnemy, manual, auto);
            ParseBlock(unconditional, auto);
            ParseDraw(unconditional, auto);
            ParseEnergy(unconditional, auto);
            ParseDiscard(unconditional, auto);
            ParseHeal(unconditional, auto);
        }

        private static string UnconditionalPart(string desc)
        {
            int index = desc.IndexOf("人剑合一");
            if (index < 0)
                index = desc.IndexOf("如果");
            if (index < 0)
                index = desc.IndexOf("每");

            return index >= 0 ? desc.Substring(0, index) : desc;
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
                auto.Add(new AutoTargetEffect(TargetType.Self, new InteractiveDiscardEffect(v)));
        }

        private static void ParseHeal(string desc, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseHeal(desc);
            if (v > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self, new DealHealEffect(v)));
        }
    }
}