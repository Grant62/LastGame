using UnityEngine;

namespace Features.Combat.Targeting.View
{
    public class ArrowView : MonoBehaviour
    {
        [SerializeField] private GameObject mArrowHead;
        [SerializeField] private LineRenderer mLineRenderer;
        [SerializeField] private float mArrowOffset = 0.8f;

        private Vector3 mStartPos;
        private Camera mMainCamera;

        private void Awake()
        {
            mMainCamera = Camera.main;
        }

        private void Update()
        {
            Vector3 mousePos = GetMouseWorldPosition();
            Vector3 direction = (mousePos - mStartPos).normalized;

            mLineRenderer.SetPosition(0, mStartPos);
            mLineRenderer.SetPosition(1, mousePos - direction * mArrowOffset);

            mArrowHead.transform.position = mousePos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            mArrowHead.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public void SetupArrow(Vector3 startPosition)
        {
            mStartPos = startPosition;
            mLineRenderer.SetPosition(0, startPosition);
            mLineRenderer.SetPosition(1, GetMouseWorldPosition());
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -mMainCamera.transform.position.z;
            return mMainCamera.ScreenToWorldPoint(mousePos);
        }
    }
}