using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class TianGangEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;

            if (!sword.IsSpinning.Value)
            {
                sword.IsSpinning.Value = true;
                sword.SpinDamage.Value += Ctx.Config.SpinBaseDamage;
                Ctx.CardSystem.DrawCards(1);
            }
            else
            {
                sword.IsSpinning.Value = false;
                sword.SpinDamage.Value = 0;
                Ctx.ResourceSystem.Gain(3);
            }
        }
    }
}