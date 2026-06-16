using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Define;
using Features.Card.Utility.Parsers;
using Features.Combat.Targeting;

namespace Features.Card.Utility
{
    public static class CardEffectFactory
    {
        public static void PopulateEffects(CardDefine define, CardData cardData)
        {
            string desc = define.Desc;
            List<Effect> manual = new();
            List<AutoTargetEffect> auto = new();

            PathEffectsParser.Parse(desc, auto);
            BasicEffectsParser.Parse(desc, define.NeedsEnemyTarget, manual, auto);
            KeywordParser.Parse(desc, cardData, auto);
            LinkEffectsParser.Parse(desc, auto);
            SpiritEffectsParser.Parse(desc, auto, cardData);
            SwordSpinParser.Parse(desc, auto);
            ConditionalEffectsParser.Parse(desc, auto);
            StatusEffectsParser.Parse(desc, define.NeedsEnemyTarget, manual, auto);

            cardData.ManualTargetEffect = manual;
            cardData.OtherEffects = auto;
        }
    }
}