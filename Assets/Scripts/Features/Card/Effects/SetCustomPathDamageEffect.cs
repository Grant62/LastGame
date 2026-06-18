using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class SetCustomPathDamageEffect : Effect
    {
        private readonly int mDamage;

        public SetCustomPathDamageEffect(int damage)
        {
            mDamage = damage;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            Ctx.SwordModel.CustomPathDamage = mDamage;
        }
    }
}