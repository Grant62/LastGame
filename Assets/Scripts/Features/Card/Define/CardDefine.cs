using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Effects;
using Features.Combat.Targeting;

namespace Features.Card.Define
{
    public struct CardDefine
    {
        public int Id;
        public string Name;
        public int Cost;
        public int Damage;
        public int Block;
        public string Desc;

        public CardData CreateCardData()
        {
            CardData card = new(
                Id, Name, 0, "Common", Desc,
                Cost, 0, "", 0,
                Damage, Block, 0,
                "", 0, "");

            if (Damage > 0)
                card.ManualTargetEffect = new List<Effect> { new DealDamageEffect(Damage) };

            if (Block > 0)
            {
                card.OtherEffects = new List<AutoTargetEffect>
                {
                    new(TargetType.Self, new DealHealEffect(Block))
                };
            }

            return card;
        }
    }
}