using QFramework;

namespace Features.Hero.Model
{
    public interface IHeroModel : IModel
    {
        BindableProperty<int> Health { get; }
        BindableProperty<int> MaxHealth { get; }
        BindableProperty<bool> Invincible { get; }
        BindableProperty<int> CurrentSlotIndex { get; }
        BindableProperty<bool> IsFacingRight { get; }
    }
}