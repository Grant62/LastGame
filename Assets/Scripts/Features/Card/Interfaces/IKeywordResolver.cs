using QFramework;

namespace Features.Card.Interfaces
{
    public interface IKeywordResolver : IUtility
    {
        string FormatDescription(string desc);
        string GetKeywordExplanations(string desc);
    }
}