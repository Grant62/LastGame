using QFramework;

namespace Features.Run.Model
{
    public interface IRunModel : IModel
    {
        BindableProperty<int> CurrentLayer { get; }

        BindableProperty<int> CurrentStep { get; }

        BindableProperty<int> ShortRestCount { get; }
    }
}