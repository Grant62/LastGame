using Features.Combat.Targeting;
using Features.Sword.Model;

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
