using Features.Combat;
using UnityEngine;

namespace Services.SlotDetector
{
    public class SlotDetector : ISlotDetector
    {
        private readonly BoardView mBoard;
        private readonly LayerMask mSlotLayer;

        public SlotDetector(BoardView board, LayerMask slotLayer)
        {
            mBoard = board;
            mSlotLayer = slotLayer;
        }

        public int GetSlotIndexAtPosition(Vector3 worldPosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, 10f, mSlotLayer);
            if (hit.collider != null)
                return mBoard.GetSlotIndex(hit.collider.transform);

            return -1;
        }
    }
}