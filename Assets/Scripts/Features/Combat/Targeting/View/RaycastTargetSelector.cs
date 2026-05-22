using UnityEngine;

namespace Features.Combat.Targeting.View
{
    public class RaycastTargetSelector : ITargetSelector
    {
        private readonly LayerMask mLayerMask;

        public RaycastTargetSelector(LayerMask layerMask)
        {
            mLayerMask = layerMask;
        }

        public ITargetable GetTargetAtPosition(Vector3 position)
        {
            if (Physics.Raycast(position, Vector3.forward, out RaycastHit hit, 10f, mLayerMask)
                && hit.collider != null
                && hit.collider.TryGetComponent(out ITargetable target)
                && target.IsValidTarget)
                return target;

            return null;
        }

        public ITargetable GetTargetAtMousePosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            return GetTargetAtPosition(worldPos);
        }
    }
}