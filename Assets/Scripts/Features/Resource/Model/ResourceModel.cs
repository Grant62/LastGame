using QFramework;

namespace Features.Resource.Model
{
    public class ResourceModel : AbstractModel, IResourceModel
    {
        public BindableProperty<int> CurEnergy { get; } = new();
        public BindableProperty<int> MaxEnergy { get; } = new(3);
        public BindableProperty<int> Gold { get; } = new(500);

        protected override void OnInit() { }
    }
}