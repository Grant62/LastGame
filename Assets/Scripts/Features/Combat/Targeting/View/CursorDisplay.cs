using UnityEngine;

namespace Features.Combat.Targeting.View
{
    public class CursorDisplay : ICursorDisplay
    {
        private readonly GameObject mCursor;

        public CursorDisplay(GameObject cursorPrefab)
        {
            mCursor = Object.Instantiate(cursorPrefab);
            mCursor.SetActive(false);
        }

        public void ShowAt(Vector3 position)
        {
            mCursor.SetActive(true);
            mCursor.transform.position = position;
        }

        public void Hide()
        {
            mCursor.SetActive(false);
        }

        public void Cleanup()
        {
            if (mCursor != null)
                Object.Destroy(mCursor);
        }
    }
}