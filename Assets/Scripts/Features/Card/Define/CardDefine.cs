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
        public int Heal;
        public string Desc;

        public CardData CreateCardData()
        {
            CardData card = new(Id, Name, 0, "Common", Desc, Cost, 0, "", 0, Damage, 0, 0, "", 0, "");

            List<Effect> effects = new();

            if (Damage > 0)
                effects.Add(new DealDamageEffect(Damage));

            if (Heal > 0)
                effects.Add(new DealHealEffect(Heal));

            if (effects.Count > 0)
                card.ManualTargetEffect = effects;

            return card;
        }
    }
}