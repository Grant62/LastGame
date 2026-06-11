using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class TurnStartSpiritSpawnEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            Ctx.SwordModel.HasTurnStartSpiritSpawn = true;
        }
    }
}