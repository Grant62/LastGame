using QFramework;

namespace Features.Resource.Model
{
    public class ResourceModel : AbstractModel, IResourceModel
    {
        public BindableProperty<int> Current { get; } = new();
        public BindableProperty<int> Max { get; } = new();
        public int PerTurnIncome { get; set; } = 1;

        protected override void OnInit() { }
    }
}