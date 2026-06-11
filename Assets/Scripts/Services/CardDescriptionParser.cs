using System.Text.RegularExpressions;

namespace Services
{
    public static class CardDescriptionParser
    {
        private static readonly Regex DamageRegex = new(@"造成(\d+)点伤害");
        private static readonly Regex BlockRegex = new(@"获得(\d+)点【?护甲】?");
        private static readonly Regex DistanceRegex = new(@"(\d+)距离");
        private static readonly Regex DrawRegex = new(@"抽(\d+)张牌");
        private static readonly Regex EnergyRegex = new(@"获得(\d+)点能量");
        private static readonly Regex DiscardRegex = new(@"弃(?:掉)?(\d+)张手牌");
        private static readonly Regex HealRegex = new(@"恢复(\d+)点生命值");
        private static readonly Regex SpinCountRegex = new(@"【旋剑】(\d+)次");
        private static readonly Regex StatusStacksRegex = new(@"施加(\d+)层");

        public static int ParseDamage(string desc)
        {
            return ParseInt(desc, DamageRegex);
        }

        public static int ParseBlock(string desc)
        {
            return ParseInt(desc, BlockRegex);
        }

        public static int ParseDistance(string desc)
        {
            return ParseInt(desc, DistanceRegex);
        }

        public static int ParseDraw(string desc)
        {
            return ParseInt(desc, DrawRegex);
        }

        public static int ParseEnergy(string desc)
        {
            return ParseInt(desc, EnergyRegex);
        }

        public static int ParseDiscard(string desc)
        {
            return ParseInt(desc, DiscardRegex);
        }

        public static int ParseHeal(string desc)
        {
            return ParseInt(desc, HealRegex);
        }

        public static int ParseSpinCount(string desc)
        {
            return ParseInt(desc, SpinCountRegex);
        }

        public static int ParseStatusStacks(string desc)
        {
            return ParseInt(desc, StatusStacksRegex);
        }

        public static bool ContainsKeyword(string desc, string keyword)
        {
            if (string.IsNullOrEmpty(desc) || string.IsNullOrEmpty(keyword))
                return false;

            return desc.Contains($"【{keyword}】");
        }

        public static bool ContainsAnyKeyword(string desc, params string[] keywords)
        {
            foreach (string kw in keywords)
            {
                if (ContainsKeyword(desc, kw))
                    return true;
            }

            return false;
        }

        private static int ParseInt(string desc, Regex regex)
        {
            if (string.IsNullOrEmpty(desc))
                return 0;

            Match match = regex.Match(desc);
            return match.Success && int.TryParse(match.Groups[1].Value, out int result) ? result : 0;
        }
    }
}