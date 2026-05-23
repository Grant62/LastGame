using System.Collections.Generic;
using QFramework;

namespace Features.Card.Define
{
    public class CardDefineModel : AbstractModel, ICardDefineModel
    {
        private readonly Dictionary<int, CardDefine> mDefines = new();

        public IReadOnlyDictionary<int, CardDefine> Defines { get => mDefines; }

        protected override void OnInit() { }

        public void Register(CardDefine define)
        {
            mDefines[define.Id] = define;
        }

        public bool TryGet(int cardId, out CardDefine define)
        {
            return mDefines.TryGetValue(cardId, out define);
        }
    }
}