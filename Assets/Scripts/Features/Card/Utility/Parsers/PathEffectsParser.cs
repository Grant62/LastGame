using System.Collections.Generic;
using Features.Card.Effects;
using Features.Combat.Targeting;
using Services;

namespace Features.Card.Utility.Parsers
{
    public static class PathEffectsParser
    {
        public static void Parse(string desc, List<AutoTargetEffect> auto)
        {
            ParsePathSuppress(desc, auto);
            ParseSwordPathDamage(desc, auto);
            ParsePurgeDesc(desc, auto);
        }

        private static void ParsePathSuppress(string desc, List<AutoTargetEffect> auto)
        {
            if (desc.Contains("但不造成途经"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.SuppressPathDamage)));
        }

        private static void ParseSwordPathDamage(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("途经") || !CardDescriptionParser.ContainsKeyword(desc, "飞剑"))
                return;

            int dmg = CardDescriptionParser.ParseDamage(desc);
            if (dmg <= 0) dmg = 7;
            auto.Add(new AutoTargetEffect(TargetType.AllEnemies, new SwordPathDamageEffect(dmg)));
        }

        private static void ParsePurgeDesc(string desc, List<AutoTargetEffect> auto)
        {
            if (desc.Contains("去除") && desc.Contains("负面"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new PurgeRandomDebuffEffect()));
        }
    }
}