using Features.Combat.Targeting;
using Features.Combat.UI.Board;

namespace Features.Card.Effects
{
    public class ApplyVulnerableEffect : Effect
    {
        private readonly int mStacks;

        public ApplyVulnerableEffect(int stacks = 1)
        {
            mStacks = stacks;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            foreach (ITargetable target in targets)
            {
                if (target is EnemyUI enemy)
                    StatusHelper.ApplyStatus(enemy.Statuses, StatusType.Vulnerable, mStacks);
            }
        }
    }
}