using QFramework;

namespace Features.Sword.Model
{
    public class SwordModel : AbstractModel, ISwordModel
    {
        public BindableProperty<int> CurrentSlotIndex { get; } = new(-1);
        public BindableProperty<bool> IsSummoned { get; } = new();
        public BindableProperty<bool> IsFollowingPlayer { get; } = new();

        protected override void OnInit() { }
    }
}