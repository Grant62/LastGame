using QFramework;

namespace Features.Sword.Model
{
    public interface ISwordModel : IModel
    {
        BindableProperty<int> CurrentSlotIndex { get; }
        BindableProperty<bool> IsSummoned { get; }
        BindableProperty<bool> IsFollowingPlayer { get; }
    }
}