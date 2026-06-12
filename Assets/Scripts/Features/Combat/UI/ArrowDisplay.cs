using Features.Combat.Targeting;
using UnityEngine;

namespace Features.Combat.UI
{
    public class ArrowDisplay : IArrowDisplay
    {
        private readonly RectTransform mCanvasRect;
        private readonly GameObject mArrowHead;
        private readonly RectTransform mLineRect;
        private readonly float mArrowOffset;
        private readonly float mLineWidth;
        private Vector2 mStartPos;
        private bool mIsActive;

        public ArrowDisplay(GameObject arrowHead, GameObject lineObj, float arrowOffset, float lineWidth = 3f)
        {
            mArrowHead = arrowHead;
            mLineRect = lineObj.GetComponent<RectTransform>();
            mArrowOffset = arrowOffset;
            mLineWidth = lineWidth;

            Canvas canvas = arrowHead.GetComponentInParent<Canvas>();
            mCanvasRect = canvas.GetComponent<RectTransform>();

            mLineRect.gameObject.SetActive(false);
            mArrowHead.SetActive(false);
        }

        public void Show(Vector3 startPosition)
        {
            mStartPos = ScreenToCanvas(startPosition);
            mLineRect.gameObject.SetActive(true);
            mArrowHead.SetActive(true);
            mIsActive = true;
        }

        public void Hide()
        {
            mIsActive = false;
            mLineRect.gameObject.SetActive(false);
            mArrowHead.SetActive(false);
        }

        public void UpdateMouse(Vector3 mousePos)
        {
            if (!mIsActive)
                return;

            Vector2 canvasMouse = ScreenToCanvas(mousePos);
            Vector2 direction = (canvasMouse - mStartPos).normalized;
            Vector2 end = canvasMouse;
            Vector2 mid = (mStartPos + end) * 0.5f;
            float length = Vector2.Distance(mStartPos, end);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            mLineRect.sizeDelta = new Vector2(length, mLineRect.sizeDelta.y);
            mLineRect.position = mid;
            mLineRect.rotation = Quaternion.Euler(0, 0, angle);

            mArrowHead.transform.position = canvasMouse;
            mArrowHead.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private Vector2 ScreenToCanvas(Vector2 screenPos)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                mCanvasRect, screenPos, null, out Vector3 worldPos);
            return worldPos;
        }
    }
}