using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;

namespace Features.Card
{
    public class KeywordResolver : IKeywordResolver
    {
        private static readonly Regex KeywordRegex = new(@"【(.+?)】");
        private readonly Dictionary<string, string> mEntries = new();
        private readonly HashSet<string> mVisited = new();

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

        public string ResolveKeywords(string desc)
        {
            if (string.IsNullOrEmpty(desc))
                return "";

            mVisited.Clear();
            StringBuilder sb = new();
            ResolveRecursive(desc, sb, 0);
            return sb.ToString();
        }

        private void ResolveRecursive(string text, StringBuilder sb, int depth)
        {
            foreach (Match match in KeywordRegex.Matches(text))
            {
                string keyword = match.Groups[1].Value;
                if (!mEntries.TryGetValue(keyword, out string keywordDesc) || !mVisited.Add(keyword))
                    continue;

                sb.Append(' ', depth * 2);
                sb.AppendLine($"【{keyword}】: {keywordDesc}");

                ResolveRecursive(keywordDesc, sb, depth + 1);
            }
        }
    }
}