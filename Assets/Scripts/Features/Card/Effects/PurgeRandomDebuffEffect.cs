using Core.Systems;
using Features.Combat.Interfaces;
using Features.Hero.Model;

namespace Features.Card.Effects
{
    public class PurgeRandomDebuffEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            IHeroModel hero = Ctx.HeroModel;
            if (hero.Statuses.Count == 0) return;

            int index = Ctx.RandomSystem.Range(0, hero.Statuses.Count, RandomModuleIds.Combat);
            hero.Statuses.RemoveAt(index);
        }
    }
}