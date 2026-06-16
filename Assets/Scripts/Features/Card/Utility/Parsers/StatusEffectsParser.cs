using System.Collections.Generic;
using Features.Card.Effects;
using Features.Combat.Targeting;
using Services;

namespace Features.Card.Utility.Parsers
{
    public static class StatusEffectsParser
    {
        public static void Parse(string desc, bool needsEnemy, List<Effect> manual, List<AutoTargetEffect> auto)
        {
            int stacks = CardDescriptionParser.ParseStatusStacks(desc);
            if (stacks <= 0) return;

            if (CardDescriptionParser.ContainsKeyword(desc, "虚弱"))
            {
                if (needsEnemy)
                    manual.Add(new ApplyStatusEffect(StatusType.Weak, stacks));
                else
                    auto.Add(new AutoTargetEffect(TargetType.RandomEnemy, new ApplyStatusEffect(StatusType.Weak, stacks)));
            }

            if (CardDescriptionParser.ContainsKeyword(desc, "易伤"))
            {
                if (needsEnemy)
                    manual.Add(new ApplyStatusEffect(StatusType.Vulnerable, stacks));
                else
                    auto.Add(new AutoTargetEffect(TargetType.RandomEnemy, new ApplyStatusEffect(StatusType.Vulnerable, stacks)));
            }
        }
    }
}