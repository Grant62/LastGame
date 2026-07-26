using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Features.Card.Utility
{
    public class KeywordResolver : IKeywordResolver
    {
        private static readonly Regex KeywordRegex = new(@"【(.+?)】");
        private readonly Dictionary<string, string> mEntries = new();

        public KeywordResolver(cfg.TbEntryInfo entryTable)
        {
            if (entryTable?.DataList == null)
                return;

            foreach (cfg.EntryInfo entry in entryTable.DataList)
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

        public string GetExplanation(string keyword)
        {
            mEntries.TryGetValue(keyword, out string explanation);
            return explanation;
        }

        public List<(string name, string desc)> CollectKeywords(string desc)
        {
            List<(string name, string desc)> result = new();
            if (string.IsNullOrEmpty(desc))
                return result;

            HashSet<string> visited = new();
            Queue<string> queue = new();

            foreach (Match match in KeywordRegex.Matches(desc))
            {
                string keyword = match.Groups[1].Value;
                if (ShouldExpand(keyword) && visited.Add(keyword))
                    queue.Enqueue(keyword);
            }

            while (queue.Count > 0)
            {
                string keyword = queue.Dequeue();
                if (!mEntries.TryGetValue(keyword, out string explanation) || string.IsNullOrEmpty(explanation))
                    continue;

                result.Add((keyword, explanation));

                foreach (Match match in KeywordRegex.Matches(explanation))
                {
                    string nested = match.Groups[1].Value;
                    if (ShouldExpand(nested) && visited.Add(nested))
                        queue.Enqueue(nested);
                }
            }

            return result;
        }

        private bool ShouldExpand(string keyword)
        {
            return mEntries.ContainsKey(keyword);
        }
    }
}