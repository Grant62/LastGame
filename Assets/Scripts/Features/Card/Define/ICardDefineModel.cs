using System.Collections.Generic;
using QFramework;

namespace Features.Card.Define
{
    public interface ICardDefineModel : IModel
    {
        IReadOnlyDictionary<int, CardDefine> Defines { get; }
        void Register(CardDefine define);
        bool TryGet(int cardId, out CardDefine define);
    }
}