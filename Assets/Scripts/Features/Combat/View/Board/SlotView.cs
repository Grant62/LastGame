using Features.Combat.Targeting;
using UnityEngine;

namespace Features.Combat.View.Board
{
    public class SlotView : MonoBehaviour, ISlotTarget
    {
        public int SlotIndex { get; set; }
        public RectTransform SlotRect { get; private set; }

        private void Awake()
        {
            SlotRect = GetComponent<RectTransform>();
        }
    }
}