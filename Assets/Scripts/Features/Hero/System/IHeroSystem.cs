using QFramework;

namespace Features.Hero.System
{
    public interface IHeroSystem : ISystem
    {
        void Setup(HeroDefine define);
        void TakeDamage(int damage);
        void Heal(int amount);
    }
}