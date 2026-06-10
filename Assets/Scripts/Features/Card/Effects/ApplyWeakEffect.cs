using Features.Combat.Targeting;
using Features.Combat.UI.Board;

namespace Features.Card.Effects
{
    public class ApplyWeakEffect : Effect
    {
        private readonly int mStacks;

        public ApplyWeakEffect(int stacks = 1)
        {
            mStacks = stacks;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            foreach (ITargetable target in targets)
            {
                if (target is EnemyUI enemy)
                    StatusHelper.ApplyStatus(enemy.Statuses, StatusType.Weak, mStacks);
            }
        }
    }
}