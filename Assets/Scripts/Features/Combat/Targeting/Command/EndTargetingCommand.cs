using Features.Combat.Targeting.Event;
using Features.Combat.Targeting.Model;
using QFramework;

namespace Features.Combat.Targeting.Command
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