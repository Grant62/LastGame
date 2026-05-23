using QFramework;

namespace Features.Enemy.Model
{
    public interface IEnemyModel : IModel
    {
        BindableProperty<int> MonsterId { get; }
        BindableProperty<int> Health { get; }
        BindableProperty<int> MaxHealth { get; }
        BindableProperty<int> Damage { get; }
    }
}