using Features.Run.Model;
using QFramework;

namespace Features.Run.Command
{
    public class ResetShortRestCountCommand : AbstractCommand
    {
        private readonly int mCount;

        public ResetShortRestCountCommand(int count)
        {
            mCount = count;
        }

        protected override void OnExecute()
        {
            this.GetModel<IRunModel>().ShortRestCount.Value = mCount;
        }
    }
}