using Features.Combat.Interfaces;

namespace Features.Card.Effects
{
    public class DrawCardsEffect : Effect
    {
        private readonly int mCount;

        public DrawCardsEffect(int count)
        {
            mCount = count;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            Ctx.CardSystem.DrawCards(mCount);
        }
    }
}