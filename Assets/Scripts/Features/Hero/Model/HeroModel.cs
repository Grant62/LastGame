using QFramework;

namespace Features.Hero.Model
{
    public class HeroModel : AbstractModel, IHeroModel
    {
        public BindableProperty<int> Health { get; } = new();
        public BindableProperty<int> MaxHealth { get; } = new();
        public BindableProperty<bool> Invincible { get; } = new();
        public BindableProperty<int> CurrentSlotIndex { get; } = new();
        public BindableProperty<bool> IsFacingRight { get; } = new(true);

        protected override void OnInit() { }
    }
}