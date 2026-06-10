using Core.Architecture;
using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class DestroySpiritSwordEffect : Effect
    {
        private readonly int mSlotIndex;

        public DestroySpiritSwordEffect(int slotIndex)
        {
            mSlotIndex = slotIndex;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = GameMain.Interface.GetModel<ISwordModel>();

            if (model.SpiritSwordSlots.Remove(mSlotIndex))
                model.OnSpiritSwordsChanged.Trigger();
        }
    }
}