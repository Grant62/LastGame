using Features.Combat.Event;
using Features.Combat.Model;
using QFramework;

namespace Features.Combat.Command
{
    public class EndTargetingCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            ITargetingModel model = this.GetModel<ITargetingModel>();
            model.IsTargeting.Value = false;

            this.SendEvent<TargetingEndedEvent>();
        }
    }
}