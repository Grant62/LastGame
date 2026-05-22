using Features.Combat.Targeting.System;
using QFramework;
using UnityEngine;

namespace Features.Combat.Targeting.Query
{
    public class GetTargetAtPositionQuery : AbstractQuery<ITargetable>
    {
        private readonly Vector3 mPosition;

        public GetTargetAtPositionQuery(Vector3 position)
        {
            mPosition = position;
        }

        protected override ITargetable OnDo()
        {
            return this.GetSystem<ITargetingSystem>().GetTargetAtPosition(mPosition);
        }
    }
}