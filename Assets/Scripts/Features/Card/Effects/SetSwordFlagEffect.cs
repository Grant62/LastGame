using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public enum SwordFlag
    {
        IsSpiritAttached,
        KeepSpinningOnMove,
        HasTurnStartSpiritSpawn,
        HasReactiveSpiritSpawn,
        SuppressPathDamage,
        SpinHitsAdjacent,
        SpinAffectsSpirits
    }

    public class SetSwordFlagEffect : Effect
    {
        private readonly SwordFlag mFlag;

        public SetSwordFlagEffect(SwordFlag flag)
        {
            mFlag = flag;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = Ctx.SwordModel;
            switch (mFlag)
            {
                case SwordFlag.IsSpiritAttached: model.IsSpiritAttached.Value = true; break;
                case SwordFlag.KeepSpinningOnMove: model.KeepSpinningOnMove = true; break;
                case SwordFlag.HasTurnStartSpiritSpawn: model.HasTurnStartSpiritSpawn = true; break;
                case SwordFlag.HasReactiveSpiritSpawn: model.HasReactiveSpiritSpawn = true; break;
                case SwordFlag.SuppressPathDamage: model.SuppressPathDamage = true; break;
                case SwordFlag.SpinHitsAdjacent: model.SpinHitsAdjacent = true; break;
                case SwordFlag.SpinAffectsSpirits: model.SpinAffectsSpirits = true; break;
            }
        }
    }
}