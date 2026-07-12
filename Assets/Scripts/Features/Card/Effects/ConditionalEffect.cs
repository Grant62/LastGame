using Features.Combat.Interfaces;
using Features.Combat.View.Board;
using Features.Enemy.View;
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
            mConditionEffect.Ctx = Ctx;
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
            {
                if (mIfTrue != null)
                {
                    mIfTrue.Ctx = Ctx;
                    mIfTrue.Execute(targets, caster);
                }
            }
            else
            {
                if (mIfFalse != null)
                {
                    mIfFalse.Ctx = Ctx;
                    mIfFalse.Execute(targets, caster);
                }
            }
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
            return model.IsSpiritAttached.Value;
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

    public class SlotHasSpiritSwordCondition : ConditionCheckEffect
    {
        public override bool Check(ITargetable[] targets, ITargetable caster)
        {
            return Ctx.SwordModel.SpiritSwordSlots.Contains(Ctx.SlotTargetIndex);
        }
    }

    public class SlotHasEnemyCondition : ConditionCheckEffect
    {
        public override bool Check(ITargetable[] targets, ITargetable caster)
        {
            BoardView board = Ctx.BoardAccess.Board;
            int slot = Ctx.SlotTargetIndex;
            if (slot < 0)
                return false;

            return board.TryGetEnemyAtSlot(slot, out EnemyView enemy) && enemy.IsValidTarget;
        }
    }

    public class SlotHasSwordCondition : ConditionCheckEffect
    {
        public override bool Check(ITargetable[] targets, ITargetable caster)
        {
            return Ctx.SwordModel.CurSlotIndex.Value >= 0
                   && Ctx.SwordModel.CurSlotIndex.Value == Ctx.SlotTargetIndex;
        }
    }

    public class NotSpinningCondition : ConditionCheckEffect
    {
        public override bool Check(ITargetable[] targets, ITargetable caster)
        {
            return !Ctx.SwordModel.IsSpinning.Value;
        }
    }

    public class PenetratedCondition : ConditionCheckEffect
    {
        public override bool Check(ITargetable[] targets, ITargetable caster)
        {
            return Ctx.SwordModel.LastLinkPenetrated;
        }
    }
}