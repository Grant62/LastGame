using System.Collections.Generic;
using Features.Enemy.View;
using UnityEngine;

namespace Features.Combat
{
    public class EnemyBoardView : MonoBehaviour
    {
        [SerializeField] private List<Transform> mSlots;
        [SerializeField] private EnemyView mEnemyPrefab;

        private static readonly int[] SlotPriority = { 1, 0, 2 };

        public List<EnemyView> EnemyViews { get; } = new();

        public EnemyView SpawnEnemy(Transform slot)
        {
            EnemyView enemyView = Instantiate(mEnemyPrefab, slot.position, slot.rotation, slot);
            EnemyViews.Add(enemyView);
            return enemyView;
        }

        public Transform GetFirstAvailableSlot()
        {
            foreach (int index in SlotPriority)
            {
                if (index < mSlots.Count && !IsSlotOccupied(mSlots[index]))
                    return mSlots[index];
            }

            return null;
        }

        private bool IsSlotOccupied(Transform slot)
        {
            foreach (EnemyView ev in EnemyViews)
            {
                if (ev != null && ev.transform.parent == slot)
                    return true;
            }

            return false;
        }

        public void RemoveEnemy(EnemyView enemyView)
        {
            EnemyViews.Remove(enemyView);
            Destroy(enemyView.gameObject);
        }
    }
}