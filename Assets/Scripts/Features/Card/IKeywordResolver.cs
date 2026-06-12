using QFramework;

namespace Features.Card
{
    public interface IKeywordResolver : IUtility
    {
        string ResolveKeywords(string desc);
    }
}