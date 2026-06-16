using Features.Card.Data;
using Features.Card.Utility;
using Services;

namespace Features.Card.Define
{
    public struct CardDefine
    {
        public int Id;
        public string Name;
        public int Cost;
        public string Type;
        public string Rarity;
        public string Desc;
        public string IconAddress;
        public int Price;

        public bool NeedsEnemyTarget { get => Type == "攻击" && CardDescriptionParser.ParseDamage(Desc) > 0; }

        public bool NeedsSlotTarget
        {
            get
            {
                if (string.IsNullOrEmpty(Desc) || Desc.Contains("随机"))
                    return false;

                return CardDescriptionParser.ContainsAnyKeyword(Desc, "御剑", "遁形")
                       || Desc.Contains("指定位置");
            }
        }

        public SlotAction SlotAction
        {
            get
            {
                if (!NeedsSlotTarget)
                    return SlotAction.None;

                if (CardDescriptionParser.ContainsKeyword(Desc, "御剑"))
                    return SlotAction.MoveSword;

                if (CardDescriptionParser.ContainsKeyword(Desc, "遁形"))
                    return SlotAction.MovePlayer;

                if (Desc.Contains("附着"))
                    return SlotAction.SpawnSpiritAtSlot;

                if (Desc.Contains("指定位置"))
                    return SlotAction.DestroySpirit;

                return SlotAction.None;
            }
        }

        public int SlotDistance
        {
            get => SlotAction == SlotAction.MovePlayer
                ? CardDescriptionParser.ParseDistance(Desc)
                : -1;
        }

        public CardData CreateCardData()
        {
            CardData cardData = new(
                Id, Name, Type, Rarity, Desc,
                Cost, Price, IconAddress,
                NeedsEnemyTarget, NeedsSlotTarget, SlotAction, SlotDistance);

            CardEffectFactory.PopulateEffects(this, cardData);

            return cardData;
        }
    }
}