using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class PerSpiritSwordEffect : Effect
    {
        private readonly Effect mPerSpiritEffect;

        public PerSpiritSwordEffect(Effect perSpiritEffect)
        {
            mPerSpiritEffect = perSpiritEffect;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = Ctx.SwordModel;
            for (int i = 0; i < model.SpiritSwordSlots.Count; i++)
                mPerSpiritEffect.Execute(targets, caster);
        }
    }
}