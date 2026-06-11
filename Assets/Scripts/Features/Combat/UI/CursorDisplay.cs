using Features.Combat.Targeting;
using UnityEngine;

namespace Features.Combat.UI
{
    public class CursorDisplay : ICursorDisplay
    {
        private readonly GameObject mGameObject;

        public CursorDisplay(GameObject gameObject)
        {
            mGameObject = gameObject;
            mGameObject.SetActive(false);
        }

        public void ShowAt(Vector3 position)
        {
            mGameObject.SetActive(true);
            position.y += 160f;
            mGameObject.transform.position = position;
        }

        public void Hide()
        {
            mGameObject.SetActive(false);
        }
    }
}