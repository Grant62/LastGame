using Features.Combat.Targeting;
using Features.Combat.UI.Board;

namespace Features.Card.Effects
{
    public class ApplyStatusEffect : Effect
    {
        private readonly StatusType mStatusType;
        private readonly int mStacks;

        public ApplyStatusEffect(StatusType statusType, int stacks)
        {
            mStatusType = statusType;
            mStacks = stacks;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            foreach (ITargetable target in targets)
            {
                if (target is EnemyUI enemy)
                    StatusHelper.ApplyStatus(enemy.Statuses, mStatusType, mStacks);
            }
        }
    }
}