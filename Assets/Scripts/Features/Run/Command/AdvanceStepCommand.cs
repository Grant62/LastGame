using Features.Run.Model;
using QFramework;

namespace Features.Run.Command
{
    public class AdvanceStepCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<IRunModel>().CurrentStep.Value++;
        }
    }
}