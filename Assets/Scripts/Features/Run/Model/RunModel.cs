using QFramework;

namespace Features.Run.Model
{
    public class RunModel : AbstractModel, IRunModel
    {
        public BindableProperty<int> CurrentLayer { get; } = new();

        public BindableProperty<int> CurrentStep { get; } = new();

        public BindableProperty<int> ShortRestCount { get; } = new();

        protected override void OnInit()
        {
            CurrentLayer.SetValueWithoutEvent(1);
            CurrentStep.SetValueWithoutEvent(1);
            ShortRestCount.SetValueWithoutEvent(2);
        }
    }
}