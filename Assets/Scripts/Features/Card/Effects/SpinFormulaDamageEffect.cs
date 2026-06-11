using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class SpinFormulaDamageEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;
            int formulaDamage = 5 * (sword.SpinDamage.Value + 1);

            foreach (ITargetable target in targets)
            {
                if (target is IDamageable d && d.IsValidTarget)
                    d.TakeDamage(formulaDamage);
            }
        }
    }
}