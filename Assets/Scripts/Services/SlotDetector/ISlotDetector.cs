using QFramework;
using UnityEngine;

namespace Services.SlotDetector
{
    public interface ISlotDetector : IUtility
    {
        int GetSlotIndexAtPosition(Vector3 worldPosition);
    }
}