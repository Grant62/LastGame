using QFramework;

namespace Features.Resource.Model
{
    public interface IResourceModel : IModel
    {
        BindableProperty<int> CurEnergy { get; }
        BindableProperty<int> MaxEnergy { get; }
        BindableProperty<int> Gold { get; }
        BindableProperty<int> Floor { get; }
    }
}