using Features.Combat.Targeting;
using UnityEngine;

namespace Features.Combat.UI.Board
{
    public class SlotUI : MonoBehaviour, ISlotTarget
    {
        public int SlotIndex { get; set; }
        public RectTransform SlotRect { get; private set; }

        private void Awake()
        {
            SlotRect = GetComponent<RectTransform>();
        }
    }
}