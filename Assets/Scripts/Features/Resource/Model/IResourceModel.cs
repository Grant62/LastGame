using QFramework;

namespace Features.Resource.Model
{
    public interface IResourceModel : IModel
    {
        BindableProperty<int> CurEnergy { get; }
        BindableProperty<int> MaxEnergy { get; }
        int PerTurnIncome { get; set; }
    }
}