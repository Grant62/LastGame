using System.Collections.Generic;
using Features.Card.Effects;
using Features.Combat.Targeting;
using Services;

namespace Features.Card.Utility.Parsers
{
    public static class SwordSpinParser
    {
        public static void Parse(string desc, List<AutoTargetEffect> auto)
        {
            ParseSpin(desc, auto);
            ParseSpinSpecial(desc, auto);
            ParseSpinImmediate(desc, auto);
            ParseDoubleSpin(desc, auto);
            ParseFormulaDamage(desc, auto);
            ParseSpinCostReduce(desc, auto);
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

            if (!CardDescriptionParser.ContainsKeyword(desc, "旋剑"))
                return;

            if (IsStartSpinCommand(desc))
                auto.Add(new AutoTargetEffect(TargetType.Self, new SpinSwordEffect()));
        }

        private static bool IsStartSpinCommand(string desc)
        {
            if (desc.Contains("含有【旋剑】"))
                return false;

            if (desc.Contains("【旋剑】。")
                || desc.Contains("。【旋剑】")
                || desc.Contains("【旋剑】后"))
                return true;

            if (desc.Contains("旋剑伤害") || desc.Contains("旋剑】伤害")
                                      || desc.Contains("旋剑状态") || desc.Contains("旋剑】状态"))
                return false;

            if (desc.Contains("处于"))
                return false;

            if (desc.Contains("对邻格"))
                return false;

            return true;
        }

        private static void ParseSpinSpecial(string desc, List<AutoTargetEffect> auto)
        {
            if (desc.Contains("不停止"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.KeepSpinningOnMove)));

            if (desc.Contains("对邻格也") && CardDescriptionParser.ContainsKeyword(desc, "旋剑"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.SpinHitsAdjacent)));

            if (desc.Contains("灵剑") && desc.Contains("一起生效"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.SpinAffectsSpirits)));
        }

        private static void ParseSpinImmediate(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("立即对剑格") && !desc.Contains("立即对"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self, new SpinDamageImmediateEffect()));
        }

        private static void ParseDoubleSpin(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("翻倍"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self, new DoubleSpinDamageEffect()));
        }

        private static void ParseFormulaDamage(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("5*(X+1)") || !desc.Contains("剑格敌人"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self, new SpinFormulaDamageEffect()));
        }

        private static void ParseSpinCostReduce(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("含有") || !CardDescriptionParser.ContainsKeyword(desc, "旋剑"))
                return;

            if (desc.Contains("费用减1"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new ReduceSpinCardCostEffect()));
        }
    }
}