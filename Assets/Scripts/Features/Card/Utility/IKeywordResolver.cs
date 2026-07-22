using System.Collections.Generic;
using QFramework;

namespace Features.Card.Utility
{
    public interface IKeywordResolver : IUtility
    {
        string FormatDescription(string desc);
        string GetExplanation(string keyword);
        List<(string name, string desc)> CollectKeywords(string desc);
    }
}