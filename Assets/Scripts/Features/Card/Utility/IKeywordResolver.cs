using QFramework;

namespace Features.Card.Utility
{
    public interface IKeywordResolver : IUtility
    {
        string FormatDescription(string desc);
        string GetKeywordExplanations(string desc);
    }
}