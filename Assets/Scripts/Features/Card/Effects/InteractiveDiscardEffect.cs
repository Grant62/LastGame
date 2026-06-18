using Features.Combat.Targeting;

namespace Features.Card.Effects
{
    public class InteractiveDiscardEffect : Effect
    {
        private readonly int mCount;

        public InteractiveDiscardEffect(int count)
        {
            mCount = count;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            Ctx.CardModel.PendingDiscardCount = mCount;
        }
    }
}