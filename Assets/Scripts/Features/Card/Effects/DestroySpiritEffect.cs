using Features.Combat.Interfaces;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class DestroySpiritEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            int slotTarget = Ctx.SlotTargetIndex;
            if (slotTarget < 0)
                return;

            ISwordModel model = Ctx.SwordModel;
            if (model.SpiritSwordSlots.Remove(slotTarget))
                model.OnSpiritSwordsChanged.Trigger();
        }
    }
}