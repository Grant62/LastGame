using Core.Architecture;
using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class SpawnSpiritSwordEffect : Effect
    {
        private readonly int mSlotIndex;

        public SpawnSpiritSwordEffect(int slotIndex)
        {
            mSlotIndex = slotIndex;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = GameMain.Interface.GetModel<ISwordModel>();

            if (!model.SpiritSwordSlots.Contains(mSlotIndex))
            {
                model.SpiritSwordSlots.Add(mSlotIndex);
                model.OnSpiritSwordsChanged.Trigger();
            }
        }
    }
}