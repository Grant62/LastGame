using UnityEngine;

namespace Features.Combat.Targeting.View
{
    public class ArrowDisplay : IArrowDisplay
    {
        private readonly ArrowView mArrowView;

        public ArrowDisplay(ArrowView arrowView)
        {
            mArrowView = arrowView;
        }

        public void Show(Vector3 startPosition)
        {
            mArrowView.gameObject.SetActive(true);
            mArrowView.SetupArrow(startPosition);
        }

        public void Hide()
        {
            mArrowView.gameObject.SetActive(false);
        }
    }
}