using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class SpawnSpiritAtSlotEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            int slot = Ctx.SlotTargetIndex;
            if (slot < 0)
                return;

            ISwordModel model = Ctx.SwordModel;
            if (model.SpiritSwordSlots.Contains(slot))
                return;

            model.SpiritSwordSlots.Add(slot);
            model.OnSpiritSwordsChanged.Trigger();
        }
    }
}