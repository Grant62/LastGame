using System.Collections.Generic;
using Features.Combat.Targeting;
using Sirenix.OdinInspector;

namespace Features.Card.Data
{
    public enum SlotAction
    {
        None,
        MoveSword,
        MovePlayer
    }

    public class CardData
    {
        [ShowInInspector]
        [ReadOnly]
        [TableColumnWidth(50)]
        public int CardId { get; }

        [ShowInInspector]
        [ReadOnly]
        [TableColumnWidth(120)]
        public string Name { get; }

        [ShowInInspector]
        [ReadOnly]
        [TableColumnWidth(50)]
        public string Type { get; }

        [ShowInInspector]
        [ReadOnly]
        [TableColumnWidth(60)]
        public string Rarity { get; }

        [ShowInInspector]
        [ReadOnly]
        [TableColumnWidth(50)]
        [LabelText("费用")]
        public int Cost { get; set; }

        [ShowInInspector]
        [ReadOnly]
        [TableColumnWidth(50)]
        [LabelText("伤害")]
        public int Damage { get; }

        [ShowInInspector]
        [ReadOnly]
        [TableColumnWidth(50)]
        [LabelText("护盾")]
        public int Shield { get; }

        [ShowInInspector]
        [ReadOnly]
        [TableColumnWidth(50)]
        [LabelText("抽牌")]
        public int Draw { get; }

        public string Desc { get; }
        public int Price { get; }
        public string Character { get; }
        public int UnlockLevel { get; }
        public int UpgradeId { get; }
        public string CardParam { get; }
        public bool IsTemp { get; set; }
        public string IconAddress { get; }

        public bool NeedsEnemyTarget { get; }
        public bool NeedsSlotTarget { get; }
        public SlotAction SlotAction { get; }
        public int SlotDistance { get; }

        public List<Effect> ManualTargetEffect { get; set; } = new();
        public List<AutoTargetEffect> OtherEffects { get; set; } = new();

        public CardData(
            int cardId,
            string name,
            string type,
            string rarity,
            string desc,
            int cost,
            int price,
            string character,
            int unlockLevel,
            int damage,
            int shield,
            int draw,
            string iconAddress,
            int upgradeId,
            string cardParam,
            bool needsEnemyTarget,
            bool needsSlotTarget,
            SlotAction slotAction,
            int slotDistance)
        {
            CardId = cardId;
            Name = name;
            Type = type;
            Rarity = rarity;
            Desc = desc;
            Cost = cost;
            Price = price;
            Character = character;
            UnlockLevel = unlockLevel;
            Damage = damage;
            Shield = shield;
            Draw = draw;
            IconAddress = iconAddress;
            UpgradeId = upgradeId;
            CardParam = cardParam;
            NeedsEnemyTarget = needsEnemyTarget;
            NeedsSlotTarget = needsSlotTarget;
            SlotAction = slotAction;
            SlotDistance = slotDistance;
        }

        public override string ToString()
        {
            return $"[{CardId}] {Name} (费用:{Cost} 伤害:{Damage} 护盾:{Shield})";
        }
    }
}