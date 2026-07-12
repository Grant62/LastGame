using System.Collections.Generic;
using Features.Card.Define;
using QFramework;

namespace Features.Card.Model
{
    public class CardDefineModel : AbstractModel, ICardDefineModel
    {
        private readonly Dictionary<int, CardDefine> mDefines = new();
        private readonly Dictionary<string, List<CardDefine>> mDefinesByRarity = new();

        public IReadOnlyDictionary<int, CardDefine> Defines { get => mDefines; }

        protected override void OnInit() { }

        public void Register(CardDefine define)
        {
            mDefines[define.Id] = define;
            if (!mDefinesByRarity.TryGetValue(define.Rarity, out List<CardDefine> list))
            {
                list = new List<CardDefine>();
                mDefinesByRarity[define.Rarity] = list;
            }

            list.Add(define);
        }

        public bool TryGet(int cardId, out CardDefine define)
        {
            return mDefines.TryGetValue(cardId, out define);
        }

        public List<CardDefine> GetDefinesByRarity(string rarity)
        {
            mDefinesByRarity.TryGetValue(rarity, out List<CardDefine> list);
            return list ?? new List<CardDefine>();
        }
    }
}