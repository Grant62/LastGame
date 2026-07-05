using QFramework;

namespace Features.Run.Model
{
    public class RunModel : AbstractModel, IRunModel
    {
        public BindableProperty<int> CurrentLayer { get; } = new();

        public BindableProperty<int> CurrentStep { get; } = new();

        protected override void OnInit()
        {
            CurrentLayer.SetValueWithoutEvent(1);
            CurrentStep.SetValueWithoutEvent(1);
        }
    }
}