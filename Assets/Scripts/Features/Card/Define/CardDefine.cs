using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Utility;

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
        public string SlotActionStr;
        public int SlotDistance;
        public EffectSlot[] EffectSlots;

        public bool NeedsEnemyTarget
        {
            get
            {
                if (EffectSlots == null)
                    return false;
                foreach (EffectSlot slot in EffectSlots)
                    if (slot is { IsEmpty: false, Target: EffectTarget.ManualEnemy })
                        return true;
                return false;
            }
        }

        public bool NeedsSlotTarget
        {
            get
            {
                if (ResolveSlotAction() != SlotAction.None)
                    return true;
                return HasAnyEffectType(EffectType.MovePlayerToSlot);
            }
        }

        public SlotAction SlotActionEnum
        {
            get => ResolveSlotAction();
        }

        private SlotAction ResolveSlotAction()
        {
            return SlotActionStr switch
            {
                "MoveSword" => SlotAction.MoveSword,
                "MovePlayer" => SlotAction.MovePlayer,
                "DestroySpirit" => SlotAction.DestroySpirit,
                "SpawnSpiritAtSlot" => SlotAction.SpawnSpiritAtSlot,
                _ => SlotAction.None
            };
        }

        public CardData CreateCardData()
        {
            CardData cardData = new(
                Id, Name, Type, Rarity, Desc,
                Cost, Price, IconAddress,
                NeedsEnemyTarget, NeedsSlotTarget, SlotActionEnum, SlotDistance);

            EffectConfigReader.PopulateEffects(EffectSlots, cardData);

            cardData.HasSpinEffect = HasAnySpinEffect();
            cardData.HasMovePlayerSlotEffect = HasAnyEffectType(EffectType.MovePlayerToSlot);
            cardData.NeedsSpiritAttachedForSlot = HasAnyCondition(EffectCondition.SpiritAttached);

            return cardData;
        }

        private bool HasAnyCondition(EffectCondition condition)
        {
            if (EffectSlots == null)
                return false;
            foreach (EffectSlot slot in EffectSlots)
                if (slot.Condition == condition)
                    return true;
            return false;
        }

        private bool HasAnySpinEffect()
        {
            if (EffectSlots == null)
                return false;
            foreach (EffectSlot slot in EffectSlots)
            {
                if (slot.IsEmpty)
                    continue;
                if (slot.Type is EffectType.SpinSword or EffectType.StopSpin or EffectType.SpinDamageImmediate
                    or EffectType.SpinDamageRandomEnemy or EffectType.DoubleSpinDamage
                    or EffectType.SpinFormulaDamage or EffectType.ReduceSpinCost
                    or EffectType.GainSpinBlock or EffectType.RecallSpinDamage)
                    return true;
            }

            return false;
        }

        public List<EffectSlot> GetActiveSlots()
        {
            List<EffectSlot> active = new();
            if (EffectSlots == null)
                return active;
            foreach (EffectSlot slot in EffectSlots)
                if (!slot.IsEmpty)
                    active.Add(slot);
            return active;
        }

        private bool HasAnyEffectType(EffectType type)
        {
            if (EffectSlots == null)
                return false;
            foreach (EffectSlot slot in EffectSlots)
                if (slot.Type == type)
                    return true;
            return false;
        }
    }
}