using Core.Systems;
using Features.Combat.Targeting;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class SpawnSpiritsAtRandomEffect : Effect
    {
        private readonly int mCount;

        public SpawnSpiritsAtRandomEffect(int count = 1)
        {
            mCount = count;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = Ctx.SwordModel;

            for (int i = 0; i < mCount; i++)
            {
                for (int attempt = 0; attempt < 20; attempt++)
                {
                    int slot = Ctx.RandomSystem.Range(0, 9, RandomModuleIds.Combat);
                    if (!model.SpiritSwordSlots.Contains(slot))
                    {
                        model.SpiritSwordSlots.Add(slot);
                        break;
                    }
                }
            }

            model.OnSpiritSwordsChanged.Trigger();
        }
    }
}