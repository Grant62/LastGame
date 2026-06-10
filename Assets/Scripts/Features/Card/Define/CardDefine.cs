using Features.Card.Data;
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
        public int UnlockLevel;
        public int UpgradeId;

        public int Damage { get => CardDescriptionParser.ParseDamage(Desc); }

        public int Block { get => CardDescriptionParser.ParseBlock(Desc); }

        public bool NeedsEnemyTarget { get => Type == "攻击" && !NeedsSlotTarget; }

        public bool NeedsSlotTarget
        {
            get
            {
                if (string.IsNullOrEmpty(Desc) || Desc.Contains("随机"))
                    return false;

                return CardDescriptionParser.ContainsAnyKeyword(Desc, "御剑", "遁形");
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
                Cost, Price, "", UnlockLevel,
                Damage, Block, 0,
                IconAddress, UpgradeId, "",
                NeedsEnemyTarget, NeedsSlotTarget, SlotAction, SlotDistance);

            CardEffectFactory.PopulateEffects(this, cardData);

            return cardData;
        }
    }
}