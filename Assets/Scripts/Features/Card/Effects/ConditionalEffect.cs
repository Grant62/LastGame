using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class ConditionalEffect : Effect
    {
        private readonly Effect mConditionEffect;
        private readonly Effect mIfTrue;
        private readonly Effect mIfFalse;

        public ConditionalEffect(Effect conditionEffect, Effect ifTrue, Effect ifFalse = null)
        {
            mConditionEffect = conditionEffect;
            mIfTrue = ifTrue;
            mIfFalse = ifFalse;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            bool result = false;

            if (mConditionEffect is ConditionCheckEffect check)
            {
                result = check.Check(targets, caster);
            }
            else
            {
                mConditionEffect.Execute(targets, caster);
                result = true;
            }

            if (result)
                mIfTrue?.Execute(targets, caster);
            else
                mIfFalse?.Execute(targets, caster);
        }
    }

    public abstract class ConditionCheckEffect : Effect
    {
        public abstract bool Check(ITargetable[] targets, ITargetable caster);

        public override void Execute(ITargetable[] targets, ITargetable caster) { }
    }

    public class HasSpiritSwordCondition : ConditionCheckEffect
    {
        public override bool Check(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = Ctx.SwordModel;
            return model.SpiritSwordSlots.Count > 0;
        }
    }

    public class SpiritAttachedCondition : ConditionCheckEffect
    {
        public override bool Check(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = Ctx.SwordModel;
            return model.IsSpiritAttached;
        }
    }

    public class IsSpinningCondition : ConditionCheckEffect
    {
        public override bool Check(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = Ctx.SwordModel;
            return model.IsSpinning.Value;
        }
    }
}