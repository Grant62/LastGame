using Features.Combat.Interfaces;

namespace Features.Card.Effects
{
    public class SetRecallSpiritsDamageEffect : Effect
    {
        private readonly int mDamage;

        public SetRecallSpiritsDamageEffect(int damage)
        {
            mDamage = damage;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            Ctx.SwordModel.RecallSpiritsDamagePerSpirit = mDamage;
        }
    }
}