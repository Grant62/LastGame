using QFramework;

namespace Features.Enemy.System
{
    public interface IEnemySystem : ISystem
    {
        void Setup(EnemyDefine define);
        void TakeDamage(int damage);
    }
}