using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Effects;
using Features.Combat.Targeting;

namespace Features.Card.Utility
{
    public static class EffectConfigReader
    {
        public static void PopulateEffects(IReadOnlyList<EffectSlot> slots, CardData cardData)
        {
            List<Effect> manual = new();
            List<AutoTargetEffect> auto = new();

            foreach (EffectSlot slot in slots)
            {
                if (slot.IsEmpty)
                    continue;

                if (slot.Type == EffectType.SpinSword)
                {
                    int repeat = ParseInt(slot.Param1, 1);
                    for (int r = 0; r < repeat; r++)
                    {
                        Effect spin = new SpinSwordEffect();
                        spin = WrapCondition(spin, slot.Condition);
                        auto.Add(new AutoTargetEffect(ToTargetType(slot.Target), spin));
                    }

                    continue;
                }

                Effect effect = CreateEffect(slot, cardData);
                if (effect == null)
                    continue;

                effect = WrapCondition(effect, slot.Condition);

                if (slot.Target == EffectTarget.ManualEnemy)
                    manual.Add(effect);
                else
                    auto.Add(new AutoTargetEffect(ToTargetType(slot.Target), effect));
            }

            cardData.ManualTargetEffect = manual;
            cardData.OtherEffects = auto;
        }

        private static Effect CreateEffect(EffectSlot slot, CardData cardData)
        {
            switch (slot.Type)
            {
                case EffectType.DealDamage:
                    return new DealDamageEffect(ParseInt(slot.Param1));

                case EffectType.GainBlock:
                    return new GainBlockEffect(ParseInt(slot.Param1));

                case EffectType.Heal:
                    return new HealEffect(ParseInt(slot.Param1));

                case EffectType.DrawCards:
                    return new DrawCardsEffect(ParseInt(slot.Param1));

                case EffectType.InteractiveDiscard:
                    return new InteractiveDiscardEffect(ParseInt(slot.Param1));

                case EffectType.GainEnergy:
                    return new GainEnergyEffect(ParseInt(slot.Param1));

                case EffectType.GainMaxEnergy:
                    return new GainMaxEnergyEffect();

                case EffectType.RestoreEnergyToMax:
                    return new RestoreEnergyToMaxEffect();

                case EffectType.ApplyStatus:
                    return new ApplyStatusEffect(ParseStatusType(slot.Param1), ParseInt(slot.Param2));

                case EffectType.SpinSword:
                    return new SpinSwordEffect();

                case EffectType.StopSpin:
                    return new StopSpinEffect();

                case EffectType.SpinDamageImmediate:
                    return new SpinDamageImmediateEffect();

                case EffectType.SpinDamageRandomEnemy:
                    return new SpinDamageRandomEnemyEffect();

                case EffectType.DoubleSpinDamage:
                    return new DoubleSpinDamageEffect();

                case EffectType.SpinFormulaDamage:
                    return new SpinFormulaDamageEffect();

                case EffectType.ReduceSpinCost:
                    return new ReduceSpinCardCostEffect();

                case EffectType.SetSwordFlag:
                    return new SetSwordFlagEffect(ParseSwordFlag(slot.Param1));

                case EffectType.SetCustomPathDamage:
                    return new SetCustomPathDamageEffect(ParseInt(slot.Param1));

                case EffectType.RecallSwords:
                    return new RecallSwordsEffect();

                case EffectType.RecallToSlot:
                    return new RecallToSlotEffect();

                case EffectType.RecallSpiritsToTarget:
                    return new RecallSpiritsToTargetEffect(ParseInt(slot.Param1));

                case EffectType.RecallSpinDamage:
                    return new RecallSpinDamageEffect();

                case EffectType.LinkSwords:
                    return new LinkSwordsEffect(ParseInt(slot.Param1));

                case EffectType.LinkSacrificeBlock:
                    return new LinkSacrificeBlockEffect(0, ParseFloat(slot.Param1));

                case EffectType.LinkDebuff:
                    return new LinkDebuffEffect(ParseStatusType(slot.Param1), ParseInt(slot.Param2));

                case EffectType.LinkSpiritDamage:
                    return new LinkSpiritDamageEffect(ParseInt(slot.Param1));

                case EffectType.PerLinkedEnemyEnergy:
                    return new PerLinkedEnemyEffect(new GainEnergyEffect(ParseInt(slot.Param1)));

                case EffectType.PerSpiritEnergy:
                    return new PerSpiritSwordEffect(new GainEnergyEffect(ParseInt(slot.Param1)));

                case EffectType.PerSpiritDraw:
                    return new PerSpiritSwordEffect(new DrawCardsEffect(ParseInt(slot.Param1)));

                case EffectType.PerSpiritDamage:
                    return new PerSpiritSwordEffect(new DealDamageEffect(ParseInt(slot.Param1)));

                case EffectType.SacrificeBlock:
                    return new SacrificeBlockForDamageEffect(ParseFloat(slot.Param1));

                case EffectType.DestroySpirit:
                    return new DestroySpiritEffect();

                case EffectType.SpawnSpiritAtSlot:
                    return new SpawnSpiritAtSlotEffect();

                case EffectType.SpawnSpiritsRandom:
                    return new SpawnSpiritsAtRandomEffect(ParseInt(slot.Param1, 1));

                case EffectType.RideSword:
                    return new RideSwordRandomEffect();

                case EffectType.MovePlayerRandom:
                    return new MovePlayerRandomEffect();

                case EffectType.ReturnToHand:
                    return new ReturnToHandEffect();

                case EffectType.Exhaust:
                    return new ExhaustEffect(cardData);

                case EffectType.SwordPathDamage:
                    return new SwordPathDamageEffect();

                case EffectType.PathDamageIfAdjacent:
                    return new PathDamageIfAdjacentEffect(ParseInt(slot.Param1));

                case EffectType.PurgeRandomDebuff:
                    return new PurgeRandomDebuffEffect();

                case EffectType.GainSpinBlock:
                    return new GainSpinBlockEffect();

                case EffectType.MovePlayerToSlot:
                    return new MovePlayerToSlotEffect();

                case EffectType.LinkSwordsRepeat:
                    return new LinkSwordsRepeatEffect(ParseInt(slot.Param1));

                case EffectType.TianGang:
                    return new TianGangEffect();

                default:
                    return null;
            }
        }

        private static Effect WrapCondition(Effect effect, EffectCondition condition)
        {
            if (condition == EffectCondition.None)
                return effect;

            ConditionCheckEffect condEffect = condition switch
            {
                EffectCondition.Penetrated => new PenetratedCondition(),
                EffectCondition.ManSwordUnity => null,
                EffectCondition.SlotHasSpiritSword => new SlotHasSpiritSwordCondition(),
                EffectCondition.SlotHasSword => new SlotHasSwordCondition(),
                EffectCondition.SlotHasEnemy => new SlotHasEnemyCondition(),
                EffectCondition.IsSpinning => new IsSpinningCondition(),
                EffectCondition.NotSpinning => new NotSpinningCondition(),
                EffectCondition.HasSpiritSword => new HasSpiritSwordCondition(),
                EffectCondition.SpiritAttached => new SpiritAttachedCondition(),
                _ => null
            };

            if (condition == EffectCondition.ManSwordUnity)
                return new ManSwordUnityEffect(effect);

            if (condEffect != null)
                return new ConditionalEffect(condEffect, effect);

            return effect;
        }

        private static TargetType ToTargetType(EffectTarget target)
        {
            return target switch
            {
                EffectTarget.Self => TargetType.Self,
                EffectTarget.AllEnemies => TargetType.AllEnemies,
                EffectTarget.RandomEnemy => TargetType.RandomEnemy,
                EffectTarget.ManualEnemy => TargetType.Self,
                _ => TargetType.Self
            };
        }

        private static int ParseInt(string value, int defaultValue = 0)
        {
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        private static float ParseFloat(string value, float defaultValue = 0f)
        {
            return float.TryParse(value, out float result) ? result : defaultValue;
        }

        private static StatusType ParseStatusType(string value)
        {
            if (value == "Weak") return StatusType.Weak;
            if (value == "Vulnerable") return StatusType.Vulnerable;
            return StatusType.Weak;
        }

        private static SwordFlag ParseSwordFlag(string value)
        {
            return value switch
            {
                "IsSpiritAttached" => SwordFlag.IsSpiritAttached,
                "KeepSpinningOnMove" => SwordFlag.KeepSpinningOnMove,
                "HasTurnStartSpiritSpawn" => SwordFlag.HasTurnStartSpiritSpawn,
                "HasReactiveSpiritSpawn" => SwordFlag.HasReactiveSpiritSpawn,
                "SuppressPathDamage" => SwordFlag.SuppressPathDamage,
                "SpinHitsAdjacent" => SwordFlag.SpinHitsAdjacent,
                "SpinAffectsSpirits" => SwordFlag.SpinAffectsSpirits,
                "LinkAlwaysPenetrate" => SwordFlag.LinkAlwaysPenetrate,
                _ => SwordFlag.IsSpiritAttached
            };
        }
    }
}