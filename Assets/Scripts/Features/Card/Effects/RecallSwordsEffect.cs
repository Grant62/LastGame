using Features.Combat.Targeting;
using Features.Hero.Model;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class RecallSwordsEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel swordModel = Ctx.SwordModel;
            IHeroModel heroModel = Ctx.HeroModel;

            swordModel.CurSlotIndex.Value = heroModel.CurSlotIndex.Value;

            if (swordModel.SpiritSwordSlots.Count > 0)
            {
                swordModel.SpiritSwordSlots.Clear();
                swordModel.OnSpiritSwordsChanged.Trigger();
            }
        }
    }
}