using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Combat.Targeting
{
    public class TargetSelector : ITargetSelector
    {
        private readonly ITargetable mCaster;
        private readonly PointerEventData mPed = new(null);
        private readonly List<RaycastResult> mResults = new();

        public TargetSelector(ITargetable caster)
        {
            mCaster = caster;
        }

        public ITargetable GetCaster()
        {
            return mCaster;
        }

        public ITargetable GetTargetAtMousePosition()
        {
            return GetTargetAtPosition(Input.mousePosition);
        }

        public ITargetable GetTargetAtPosition(Vector3 position)
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
                return null;

            mPed.position = position;
            mResults.Clear();
            eventSystem.RaycastAll(mPed, mResults);

            ITargetable best = null;
            int bestDepth = -1;
            foreach (RaycastResult result in mResults)
            {
                IEnemyTarget enemy = result.gameObject.GetComponentInParent<IEnemyTarget>();
                if (enemy != null && enemy.IsValidTarget)
                {
                    if (best == null || result.depth > bestDepth)
                    {
                        best = enemy;
                        bestDepth = result.depth;
                    }
                }
            }

            return best;
        }
    }
}