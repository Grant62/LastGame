using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class DealDamageEffect : Effect
    {
        private readonly int mAmount;

        public DealDamageEffect(int amount)
        {
            mAmount = amount;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            foreach (ITargetable target in targets)
            {
                if (target != null && target.IsValidTarget)
                {
                    // 由项目方实现具体伤害逻辑
                    // 示例: target.TakeDamage(mAmount);
                }
            }
        }
    }
}