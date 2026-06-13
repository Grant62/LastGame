using QFramework;

namespace Features.Card.Interfaces
{
    public interface IKeywordResolver : IUtility
    {
        string ResolveKeywords(string desc);
    }
}