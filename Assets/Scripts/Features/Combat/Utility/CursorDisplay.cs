using UnityEngine;

namespace Features.Combat.Utility
{
    public class CursorDisplay : ICursorDisplay
    {
        private readonly GameObject mGameObject;
        private readonly float mHeightOffset;

        public CursorDisplay(GameObject gameObject, float heightOffset = 275f)
        {
            mGameObject = gameObject;
            mHeightOffset = heightOffset;
            mGameObject.SetActive(false);
        }

        public void ShowAt(Vector3 position)
        {
            mGameObject.SetActive(true);
            position.y += mHeightOffset;
            mGameObject.transform.position = position;
        }

        public void Hide()
        {
            mGameObject.SetActive(false);
        }
    }
}