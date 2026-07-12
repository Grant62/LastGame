using Features.Combat.Interfaces;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public enum SwordAbilityFlag
    {
        IsSpiritAttached,
        KeepSpinningOnMove,
        HasTurnStartSpiritSpawn,
        HasReactiveSpiritSpawn,
        SuppressPathDamage,
        SpinHitsAdjacent,
        SpinAffectsSpirits,
        LinkAlwaysPenetrate,
        RecallSpiritsOnSwordMove
    }

    public class SetSwordFlagEffect : Effect
    {
        private readonly SwordAbilityFlag _mAbilityFlag;

        public SetSwordFlagEffect(SwordAbilityFlag abilityFlag)
        {
            _mAbilityFlag = abilityFlag;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = Ctx.SwordModel;
            switch (_mAbilityFlag)
            {
                case SwordAbilityFlag.IsSpiritAttached: model.IsSpiritAttached.Value = true; break;
                case SwordAbilityFlag.KeepSpinningOnMove: model.KeepSpinningOnMove = true; break;
                case SwordAbilityFlag.HasTurnStartSpiritSpawn: model.HasTurnStartSpiritSpawn = true; break;
                case SwordAbilityFlag.HasReactiveSpiritSpawn: model.HasReactiveSpiritSpawn = true; break;
                case SwordAbilityFlag.SuppressPathDamage: model.SuppressPathDamage = true; break;
                case SwordAbilityFlag.SpinHitsAdjacent: model.SpinHitsAdjacent = true; break;
                case SwordAbilityFlag.SpinAffectsSpirits: model.SpinAffectsSpirits = true; break;
                case SwordAbilityFlag.LinkAlwaysPenetrate: model.LinkAlwaysPenetrate = true; break;
                case SwordAbilityFlag.RecallSpiritsOnSwordMove: model.RecallSpiritsOnSwordMove = true; break;
            }
        }
    }
}