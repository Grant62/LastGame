using QFramework;

namespace Features.Resource.Model
{
    public class ResourceModel : AbstractModel, IResourceModel
    {
        public BindableProperty<int> CurEnergy { get; } = new();
        public BindableProperty<int> MaxEnergy { get; } = new();
        public BindableProperty<int> Gold { get; } = new(500);
        public BindableProperty<int> Floor { get; } = new(1);
        public int PerTurnIncome { get; set; } = 3;

        protected override void OnInit() { }
    }
}