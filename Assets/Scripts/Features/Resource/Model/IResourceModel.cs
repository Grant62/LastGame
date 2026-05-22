using QFramework;

namespace Features.Resource.Model
{
    public interface IResourceModel : IModel
    {
        BindableProperty<int> Current { get; }
        BindableProperty<int> Max { get; }
        int PerTurnIncome { get; set; }
    }
}