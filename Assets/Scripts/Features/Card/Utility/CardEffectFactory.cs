using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Define;
using Features.Card.Effects;
using Features.Combat.Targeting;
using Services;

namespace Features.Card.Utility
{
    public static class CardEffectFactory
    {
        public static void PopulateEffects(CardDefine define, CardData cardData)
        {
            string desc = define.Desc;
            List<Effect> manual = new();
            List<AutoTargetEffect> auto = new();

            ParsePathSuppress(desc, auto);
            ParseDamage(desc, define.NeedsEnemyTarget, manual, auto);
            ParseBlock(desc, auto);
            ParseDraw(desc, auto);
            ParseEnergy(desc, auto);
            ParseDiscard(desc, auto);
            ParseHeal(desc, auto);

            ParseKeywordRecall(desc, auto);
            ParseKeywordExhaust(desc, cardData, auto);
            ParseLinkAll(desc, auto);
            ParseLinkSpiritDamage(desc, auto);
            ParseSpawnSpiritsRandom(desc, auto);
            ParseTurnStartSpiritSpawn(desc, auto);
            ParseReactiveSpiritSpawn(desc, auto);
            ParseLinkDebuff(desc, auto);
            ParsePurgeDesc(desc, auto);
            ParseSpin(desc, auto);
            ParseSpinSpecial(desc, auto);
            ParseSpinImmediate(desc, auto);
            ParseDoubleSpin(desc, auto);
            ParseSpiritAttach(desc, auto);
            ParseManSwordUnity(desc, auto);
            ParseConditional(desc, auto);
            ParseSwordSpinToEnemy(desc, manual, define.NeedsEnemyTarget);
            ParseFormulaDamage(desc, manual, define.NeedsEnemyTarget);
            ParseHealIfSpirit(desc, auto);
            ParseSwordPathDamage(desc, auto);
            ParsePerSpiritReward(desc, auto, cardData);
            ParseSacrificeBlock(desc, auto);
            ParseStatus(desc, define.NeedsEnemyTarget, manual, auto);

            cardData.ManualTargetEffect = manual;
            cardData.OtherEffects = auto;
        }

        private static void ParseDamage(string desc, bool needsEnemy, List<Effect> manual, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseDamage(desc);
            if (v <= 0) return;

            if (needsEnemy)
                manual.Add(new DealDamageEffect(v));
            else
                auto.Add(new AutoTargetEffect(TargetType.AllEnemies, new DealDamageEffect(v)));
        }

        private static void ParseBlock(string desc, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseBlock(desc);
            if (v > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self, new GainBlockEffect(v)));
        }

        private static void ParseDraw(string desc, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseDraw(desc);
            if (v > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self, new DrawCardsEffect(v)));
        }

        private static void ParseEnergy(string desc, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseEnergy(desc);
            if (v > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self, new GainEnergyEffect(v)));
        }

        private static void ParseDiscard(string desc, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseDiscard(desc);
            if (v > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self, new DiscardCardsEffect(v)));
        }

        private static void ParseHeal(string desc, List<AutoTargetEffect> auto)
        {
            int v = CardDescriptionParser.ParseHeal(desc);
            if (v > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self, new DealHealEffect(v)));
        }

        private static void ParseKeywordRecall(string desc, List<AutoTargetEffect> auto)
        {
            if (CardDescriptionParser.ContainsKeyword(desc, "剑来"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new RecallSwordsEffect()));
        }

        private static void ParseLinkAll(string desc, List<AutoTargetEffect> auto)
        {
            if (!CardDescriptionParser.ContainsKeyword(desc, "链接"))
                return;

            int customBlock = CardDescriptionParser.ParseBlock(desc);
            if (customBlock <= 0 || customBlock == 8)
                customBlock = desc.Contains("不获得") ? 0 : 8;

            Effect penetration = ParseLinkPenetrationReward(desc);
            auto.Add(new AutoTargetEffect(TargetType.Self, new LinkSwordsEffect(customBlock, penetration)));
        }

        private static Effect ParseLinkPenetrationReward(string desc)
        {
            string[] parts = desc.Split('。');
            foreach (string part in parts)
            {
                if (!part.Contains("穿透"))
                    continue;

                if (CardDescriptionParser.ParseEnergy(part) > 0)
                    return new GainEnergyEffect(CardDescriptionParser.ParseEnergy(part));
                if (CardDescriptionParser.ParseDraw(part) > 0)
                    return new DrawCardsEffect(CardDescriptionParser.ParseDraw(part));
                if (CardDescriptionParser.ParseDamage(part) > 0)
                    return new DealDamageEffect(CardDescriptionParser.ParseDamage(part));
                if (CardDescriptionParser.ParseBlock(part) > 0)
                    return new GainBlockEffect(CardDescriptionParser.ParseBlock(part));
            }

            return null;
        }

        private static void ParseKeywordExhaust(string desc, CardData cardData, List<AutoTargetEffect> auto)
        {
            if (CardDescriptionParser.ContainsKeyword(desc, "消耗"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new ExhaustEffect(cardData)));
        }

        private static void ParseSpin(string desc, List<AutoTargetEffect> auto)
        {
            int count = CardDescriptionParser.ParseSpinCount(desc);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                    auto.Add(new AutoTargetEffect(TargetType.Self, new SpinSwordEffect()));
                return;
            }

            if (CardDescriptionParser.ContainsKeyword(desc, "旋剑"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new SpinSwordEffect()));
        }

        private static void ParseStatus(string desc, bool needsEnemy, List<Effect> manual, List<AutoTargetEffect> auto)
        {
            int stacks = CardDescriptionParser.ParseStatusStacks(desc);
            if (stacks <= 0) return;

            if (CardDescriptionParser.ContainsKeyword(desc, "虚弱"))
            {
                if (needsEnemy)
                    manual.Add(new ApplyStatusEffect(StatusType.Weak, stacks));
                else
                    auto.Add(new AutoTargetEffect(TargetType.RandomEnemy, new ApplyStatusEffect(StatusType.Weak, stacks)));
            }

            if (CardDescriptionParser.ContainsKeyword(desc, "易伤"))
            {
                if (needsEnemy)
                    manual.Add(new ApplyStatusEffect(StatusType.Vulnerable, stacks));
                else
                    auto.Add(new AutoTargetEffect(TargetType.RandomEnemy, new ApplyStatusEffect(StatusType.Vulnerable, stacks)));
            }
        }

        private static void ParseSpinSpecial(string desc, List<AutoTargetEffect> auto)
        {
            if (CardDescriptionParser.ContainsKeyword(desc, "不停止"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.KeepSpinningOnMove)));

            if (desc.Contains("邻格") && CardDescriptionParser.ContainsKeyword(desc, "旋剑"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.SpinHitsAdjacent)));
        }

        private static void ParseSpiritAttach(string desc, List<AutoTargetEffect> auto)
        {
            if (CardDescriptionParser.ContainsKeyword(desc, "附着·灵")
                || desc.Contains("附着·灵"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.IsSpiritAttached)));
        }

        private static void ParsePurgeDesc(string desc, List<AutoTargetEffect> auto)
        {
            if (desc.Contains("去除") && desc.Contains("负面"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new PurgeRandomDebuffEffect()));
        }

        private static void ParseManSwordUnity(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("人剑合一"))
                return;

            Effect child = null;
            if (CardDescriptionParser.ParseEnergy(desc) > 0)
                child = new GainEnergyEffect(CardDescriptionParser.ParseEnergy(desc));
            else if (CardDescriptionParser.ParseDamage(desc) > 0)
                child = new DealDamageEffect(CardDescriptionParser.ParseDamage(desc));
            else if (CardDescriptionParser.ParseBlock(desc) > 0)
                child = new GainBlockEffect(CardDescriptionParser.ParseBlock(desc));

            if (child != null)
                auto.Add(new AutoTargetEffect(TargetType.Self, new ManSwordUnityEffect(child)));
        }

        private static void ParseConditional(string desc, List<AutoTargetEffect> auto)
        {
            if (desc.Contains("如果处于旋剑") && desc.Contains("停止且获得"))
            {
                int energy = CardDescriptionParser.ParseEnergy(desc);
                auto.Add(new AutoTargetEffect(TargetType.Self,
                    new ConditionalEffect(new IsSpinningCondition(),
                        new GainEnergyEffect(energy),
                        new SpinSwordEffect())));
            }
        }

        private static void ParseSwordSpinToEnemy(string desc, List<Effect> manual, bool needsEnemy)
        {
            if (!desc.Contains("旋剑伤害") && !desc.Contains("完整的"))
                return;

            if (needsEnemy)
                manual.Add(new SwordSpinDamageToEnemyEffect());
        }

        private static void ParseSacrificeBlock(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("消耗所有") || !desc.Contains("护甲"))
                return;

            float ratio = desc.Contains("恢复一半") ? 0.5f : 0f;
            auto.Add(new AutoTargetEffect(TargetType.AllEnemies, new SacrificeBlockForDamageEffect(ratio)));
        }

        private static void ParseFormulaDamage(string desc, List<Effect> manual, bool needsEnemy)
        {
            if (!desc.Contains("5*") || !desc.Contains("X+1"))
                return;

            if (needsEnemy)
                manual.Add(new SpinFormulaDamageEffect());
        }

        private static void ParseHealIfSpirit(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("附着") || !desc.Contains("恢复"))
                return;

            int heal = CardDescriptionParser.ParseHeal(desc);
            if (heal <= 0) heal = CardDescriptionParser.ParseBlock(desc);
            if (heal > 0)
                auto.Add(new AutoTargetEffect(TargetType.Self, new HealIfSpiritAttachedEffect(heal)));
        }

        private static void ParseSwordPathDamage(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("途经") || !CardDescriptionParser.ContainsKeyword(desc, "飞剑"))
                return;

            int dmg = CardDescriptionParser.ParseDamage(desc);
            if (dmg <= 0) dmg = 7;
            auto.Add(new AutoTargetEffect(TargetType.AllEnemies, new SwordPathDamageEffect(dmg)));
        }

        private static void ParseLinkSpiritDamage(string desc, List<AutoTargetEffect> auto)
        {
            if (!CardDescriptionParser.ContainsKeyword(desc, "链接") || !CardDescriptionParser.ContainsKeyword(desc, "灵剑"))
                return;

            int dmg = CardDescriptionParser.ParseDamage(desc);
            if (dmg <= 0) dmg = 6;
            auto.Add(new AutoTargetEffect(TargetType.Self, new LinkSpiritDamageEffect(dmg)));
        }

        private static void ParseSpawnSpiritsRandom(string desc, List<AutoTargetEffect> auto)
        {
            if (desc.Contains("随机") && desc.Contains("空位置") && CardDescriptionParser.ContainsAnyKeyword(desc, "灵剑", "生成"))
            {
                int count = 1;
                if (desc.Contains("3处")) count = 3;

                auto.Add(new AutoTargetEffect(TargetType.Self, new SpawnSpiritsAtRandomEffect(count)));
            }
        }

        private static void ParseLinkDebuff(string desc, List<AutoTargetEffect> auto)
        {
            if (!CardDescriptionParser.ContainsKeyword(desc, "链接") || !desc.Contains("施加"))
                return;

            int stacks = CardDescriptionParser.ParseStatusStacks(desc);
            if (stacks <= 0) stacks = 2;

            if (CardDescriptionParser.ContainsKeyword(desc, "虚弱"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new LinkDebuffEffect(StatusType.Weak, stacks)));

            if (CardDescriptionParser.ContainsKeyword(desc, "易伤"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new LinkDebuffEffect(StatusType.Vulnerable, stacks)));
        }

        private static void ParseTurnStartSpiritSpawn(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("每回合开始") || !CardDescriptionParser.ContainsKeyword(desc, "灵剑"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.HasTurnStartSpiritSpawn)));
        }

        private static void ParseReactiveSpiritSpawn(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("每摧毁") || !desc.Contains("空位置"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.HasReactiveSpiritSpawn)));
        }

        private static void ParseSpinImmediate(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("立即对剑格") && !desc.Contains("立即对"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self, new SpinDamageImmediateEffect()));
        }

        private static void ParseDoubleSpin(string desc, List<AutoTargetEffect> auto)
        {
            if (!desc.Contains("翻倍"))
                return;

            auto.Add(new AutoTargetEffect(TargetType.Self, new DoubleSpinDamageEffect()));
        }

        private static void ParsePerSpiritReward(string desc, List<AutoTargetEffect> auto, CardData cardData)
        {
            if (!desc.Contains("每") || !desc.Contains("灵剑"))
                return;

            Effect reward = null;
            if (CardDescriptionParser.ParseEnergy(desc) > 0)
                reward = new GainEnergyEffect(CardDescriptionParser.ParseEnergy(desc));
            else if (CardDescriptionParser.ParseDraw(desc) > 0)
                reward = new DrawCardsEffect(CardDescriptionParser.ParseDraw(desc));
            else if (CardDescriptionParser.ParseDamage(desc) > 0)
                reward = new DealDamageEffect(CardDescriptionParser.ParseDamage(desc));

            if (reward != null)
                auto.Add(new AutoTargetEffect(TargetType.Self, new PerSpiritSwordEffect(reward)));
        }

        private static void ParsePathSuppress(string desc, List<AutoTargetEffect> auto)
        {
            if (desc.Contains("但不造成途经"))
                auto.Add(new AutoTargetEffect(TargetType.Self, new SetSwordFlagEffect(SwordFlag.SuppressPathDamage)));
        }
    }
}