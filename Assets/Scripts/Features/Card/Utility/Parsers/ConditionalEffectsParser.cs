using System.Collections.Generic;
using Features.Card.Effects;
using Features.Combat.Targeting;
using Services;

namespace Features.Card.Utility.Parsers
{
    public static class ConditionalEffectsParser
    {
        public static void Parse(string desc, List<AutoTargetEffect> auto)
        {
            ParseConditional(desc, auto);
            ParseManSwordUnity(desc, auto);
            ParseSacrificeBlock(desc, auto);
            ParseRideSwordRandom(desc, auto);
            ParseMovePlayerRandom(desc, auto);
            ParseEnemyShiftBlock(desc, auto);
            ParseEnemyShiftRecall(desc, auto);
            ParseMoveToSpiritBlock(desc, auto);
        }

        private static void ParseConditional(string desc, List<AutoTargetEffect> auto)
        {
            if (desc.Contains("如果处于旋剑") && desc.Contains("停止且获得"))
            {
                int energy = CardDescriptionParser.ParseEnergy(desc);
                auto.Add(new AutoTargetEffect(TargetType.Self,
                    new ConditionalEffect(new IsSpinningCondition(),
                        new GainEnergyEffect(energy),
                        new SpinSwordEffect())));
            }
        }

        private static void ParseManSwordUnity(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("人剑合一"))
                return;

            Effect child = null;
            if (CardDescriptionParser.ParseEnergy(desc) > 0)
                child = new GainEnergyEffect(CardDescriptionParser.ParseEnergy(desc));
            else if (CardDescriptionParser.ParseDamage(desc) > 0)
                child = new DealDamageEffect(CardDescriptionParser.ParseDamage(desc));
            else if (CardDescriptionParser.ParseBlock(desc) > 0)
                child = new GainBlockEffect(CardDescriptionParser.ParseBlock(desc));

            if (child != null)
                auto.Add(new AutoTargetEffect(TargetType.Self, new ManSwordUnityEffect(child)));
        }

        private static void ParseSacrificeBlock(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("消耗所有") || !desc.Contains("护甲"))
                return;

            float ratio = desc.Contains("恢复一半") ? 0.5f : 0f;
            auto.Add(new AutoTargetEffect(TargetType.AllEnemies, new SacrificeBlockForDamageEffect(ratio)));
        }

        private static void ParseRideSwordRandom(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("御剑") || !desc.Contains("随机位置"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self, new RideSwordRandomEffect()));
        }

        private static void ParseMovePlayerRandom(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("遁形") || !desc.Contains("随机位置"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self, new MovePlayerRandomEffect()));
        }

        private static void ParseEnemyShiftBlock(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("易位"))
                return;

            int block = CardDescriptionParser.ParseBlock(desc);
            if (block > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self,
                    new ConditionalEffect(new SlotHasEnemyCondition(),
                        new GainBlockEffect(block))));
        }

        private static void ParseEnemyShiftRecall(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("易位") || !desc.Contains("剑来"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self,
                new ConditionalEffect(new SlotHasEnemyCondition(), new RecallToSlotEffect())));
        }

        private static void ParseMoveToSpiritBlock(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("移动到") || !desc.Contains("灵剑"))
                return;

            int block = CardDescriptionParser.ParseBlock(desc);
            if (block > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self,
                    new ConditionalEffect(new SlotHasSpiritSwordCondition(),
                        new GainBlockEffect(block))));

            auto.Add(new AutoTargetEffect(TargetType.Self,
                new ConditionalEffect(new SlotHasSpiritSwordCondition(),
                    new DestroySpiritEffect())));
        }
    }
}