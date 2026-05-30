using System.Linq;
using UnityEngine;

namespace Features.Combat.Targeting.View
{
    public class RaycastTargetSelector : ITargetSelector
    {
        private readonly LayerMask mLayerMask;
        private readonly Camera mMainCamera;

        public RaycastTargetSelector(LayerMask layerMask)
        {
            mLayerMask = layerMask;
            mMainCamera = Camera.main;
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
            mousePos.z = -mMainCamera.transform.position.z;
            Vector3 worldPos = mMainCamera.ScreenToWorldPoint(mousePos);
            return GetTargetAtPosition(worldPos);
        }

        public ITargetable GetCaster()
        {
            return Object.FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None
            ).OfType<IDamageable>().FirstOrDefault(d =>
                d.IsValidTarget && (mLayerMask & 1 << ((MonoBehaviour)d).gameObject.layer) == 0);
        }
    }
}