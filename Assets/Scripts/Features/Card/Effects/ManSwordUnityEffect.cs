using Features.Combat.Targeting;
using Features.Hero.Model;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class ManSwordUnityEffect : Effect
    {
        private readonly Effect mChildEffect;

        public ManSwordUnityEffect(Effect childEffect)
        {
            mChildEffect = childEffect;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;
            IHeroModel hero = Ctx.HeroModel;

            if (hero.CurSlotIndex.Value == sword.CurSlotIndex.Value)
            {
                mChildEffect.Ctx = Ctx;
                mChildEffect.Execute(targets, caster);
            }
        }
    }
}