using Core.Architecture;
using Features.Combat.Targeting;
using Features.Hero.Model;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class RecallSwordsEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel swordModel = GameMain.Interface.GetModel<ISwordModel>();
            IHeroModel heroModel = GameMain.Interface.GetModel<IHeroModel>();

            swordModel.CurSlotIndex.Value = heroModel.CurSlotIndex.Value;

            if (swordModel.SpiritSwordSlots.Count > 0)
            {
                swordModel.SpiritSwordSlots.Clear();
                swordModel.OnSpiritSwordsChanged.Trigger();
            }
        }
    }
}