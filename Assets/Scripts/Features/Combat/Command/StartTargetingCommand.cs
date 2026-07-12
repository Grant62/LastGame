using Features.Combat.Event;
using Features.Combat.Model;
using QFramework;
using UnityEngine;

namespace Features.Combat.Command
{
    public class StartTargetingCommand : AbstractCommand
    {
        private readonly Vector3 mStartPosition;

        public StartTargetingCommand(Vector3 startPosition)
        {
            mStartPosition = startPosition;
        }

        protected override void OnExecute()
        {
            ITargetingModel model = this.GetModel<ITargetingModel>();
            model.IsTargeting.Value = true;
            model.StartPosition = mStartPosition;

            this.SendEvent(new TargetingStartedEvent(mStartPosition));
        }
    }
}