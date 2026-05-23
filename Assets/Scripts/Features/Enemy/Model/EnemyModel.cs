using QFramework;

namespace Features.Enemy.Model
{
    public class EnemyModel : AbstractModel, IEnemyModel
    {
        public BindableProperty<int> MonsterId { get; } = new();
        public BindableProperty<int> Health { get; } = new();
        public BindableProperty<int> MaxHealth { get; } = new();
        public BindableProperty<int> Damage { get; } = new();

        protected override void OnInit() { }
    }
}