using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class HealIfSpiritAttachedEffect : Effect
    {
        private readonly int mAmount;

        public HealIfSpiritAttachedEffect(int amount)
        {
            mAmount = amount;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = Ctx.SwordModel;
            if (model.IsSpiritAttached.Value && caster is IDamageable d)
                d.TakeHeal(mAmount);
        }
    }
}