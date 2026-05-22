using Features.Combat.Targeting.System;
using QFramework;

namespace Features.Combat.Targeting.Query
{
    public class GetTargetAtMouseQuery : AbstractQuery<ITargetable>
    {
        protected override ITargetable OnDo()
        {
            return this.GetSystem<ITargetingSystem>().GetTargetAtMousePosition();
        }
    }
}