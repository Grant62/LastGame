using System.Text.RegularExpressions;

namespace Services
{
    public static class CardDescriptionParser
    {
        private static readonly Regex DistanceRegex = new(@"(\d+)距离");
        private static readonly Regex DrawRegex = new(@"抽(\d+)张牌");
        private static readonly Regex DiscardRegex = new(@"弃(?:掉)?(\d+)张手牌");
        private static readonly Regex SpinCountRegex = new(@"【旋剑】(\d+)次");
        private static readonly Regex StatusStacksRegex = new(@"施加(\d+)层");
        private static readonly Regex ValueRegex = new(
            @"(造成|获得|施加|恢复)(\d+)点【?(伤害|护甲|能量|生命值)】?");

        public static int ParseValue(string desc, string type)
        {
            if (string.IsNullOrEmpty(desc))
                return 0;

            foreach (Match match in ValueRegex.Matches(desc))
            {
                if (match.Groups[3].Value == type
                    && int.TryParse(match.Groups[2].Value, out int result))
                    return result;
            }

            return 0;
        }

        public static int ParseDamage(string desc)
        {
            return ParseValue(desc, "伤害");
        }

        public static int ParseBlock(string desc)
        {
            return ParseValue(desc, "护甲");
        }

        public static int ParseEnergy(string desc)
        {
            return ParseValue(desc, "能量");
        }

        public static int ParseHeal(string desc)
        {
            return ParseValue(desc, "生命值");
        }

        public static int ParseDistance(string desc)
        {
            return ParseInt(desc, DistanceRegex);
        }

        public static int ParseDraw(string desc)
        {
            return ParseInt(desc, DrawRegex);
        }

        public static int ParseDiscard(string desc)
        {
            return ParseInt(desc, DiscardRegex);
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