using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;

namespace Features.Card.Utility
{
    public class KeywordResolver : IKeywordResolver
    {
        private static readonly Regex KeywordRegex = new(@"【(.+?)】");
        private readonly Dictionary<string, string> mEntries = new();

        public KeywordResolver(EntryInfoContainer container)
        {
            if (container?.DataDic == null)
                return;

            foreach (EntryInfo entry in container.DataDic.Values)
            {
                if (!string.IsNullOrEmpty(entry.Name))
                    mEntries[entry.Name] = entry.Desc ?? "";
            }
        }

        public string FormatDescription(string desc)
        {
            if (string.IsNullOrEmpty(desc))
                return "";

            return KeywordRegex.Replace(desc, "<color=#EFC851>$1</color>");
        }

        public string GetKeywordExplanations(string desc)
        {
            if (string.IsNullOrEmpty(desc))
                return "";

            HashSet<string> visited = new();
            StringBuilder sb = new();
            foreach (Match match in KeywordRegex.Matches(desc))
            {
                string keyword = match.Groups[1].Value;
                if (mEntries.TryGetValue(keyword, out string explanation) && visited.Add(keyword))
                    sb.AppendLine($"<color=#EFC851>【{keyword}】</color> {explanation}");
            }

            return sb.Length > 0 ? sb.ToString().TrimEnd() : "";
        }
    }
}