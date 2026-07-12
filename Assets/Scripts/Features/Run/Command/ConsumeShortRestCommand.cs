using Features.Run.Model;
using QFramework;

namespace Features.Run.Command
{
    public class ConsumeShortRestCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<IRunModel>().ShortRestCount.Value--;
        }
    }
}