using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class ReactiveSpiritSpawnEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            Ctx.SwordModel.HasReactiveSpiritSpawn = true;
        }
    }
}