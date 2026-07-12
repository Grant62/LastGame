using System.Collections.Generic;
using Features.Card.Define;
using QFramework;

namespace Features.Card.Model
{
    public interface ICardDefineModel : IModel
    {
        IReadOnlyDictionary<int, CardDefine> Defines { get; }
        void Register(CardDefine define);
        bool TryGet(int cardId, out CardDefine define);
        List<CardDefine> GetDefinesByRarity(string rarity);
    }
}