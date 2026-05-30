using System.Text.RegularExpressions;

namespace Services
{
    public static class CardDescriptionParser
    {
        private static readonly Regex DamageRegex = new(@"造成(\d+)点伤害");
        private static readonly Regex BlockRegex = new(@"获得(\d+)点格挡");
        private static readonly Regex DistanceRegex = new(@"(\d+)格");

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

        private static int ParseInt(string desc, Regex regex)
        {
            Match match = regex.Match(desc);
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }
    }
}