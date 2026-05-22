using QFramework;

namespace Features.Hero.Model
{
    public class HeroModel : AbstractModel, IHeroModel
    {
        public BindableProperty<int> Health { get; } = new();
        public BindableProperty<int> MaxHealth { get; } = new();
        public BindableProperty<bool> Invincible { get; } = new();

        protected override void OnInit() { }
    }
}