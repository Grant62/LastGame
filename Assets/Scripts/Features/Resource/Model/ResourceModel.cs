using QFramework;

namespace Features.Resource.Model
{
    public class ResourceModel : AbstractModel, IResourceModel
    {
        public BindableProperty<int> CurEnergy { get; } = new();
        public BindableProperty<int> MaxEnergy { get; } = new();
        public int PerTurnIncome { get; set; } = 3;

        protected override void OnInit() { }
    }
}