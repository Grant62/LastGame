using System;
using System.Collections.Generic;
using DG.Tweening;
using Features.Enemy.View;
using UnityEngine;

namespace Features.Combat
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private List<Transform> slots;
        [SerializeField] private EnemyView enemyPrefab;
        [SerializeField] private int maxEnemyCount = 5;
        [SerializeField] private float moveDuration = 0.3f;

        private int[] mPriorityQueue;

        public List<EnemyView> EnemyViews { get; } = new();

        public int PlayerSlotIndex { get => slots.Count / 2; }

        private void Awake()
        {
            BuildPriorityQueue();
        }

        private void BuildPriorityQueue()
        {
            int slotCount = slots.Count;
            int playerIndex = slotCount / 2;
            mPriorityQueue = new int[slotCount - 1];

            int idx = 0;
            for (int offset = 1; offset <= playerIndex; offset++)
            {
                mPriorityQueue[idx++] = playerIndex - offset;

                if (playerIndex + offset < slotCount)
                    mPriorityQueue[idx++] = playerIndex + offset;
            }
        }

        public int SlotCount { get => slots.Count; }

        public float MoveDuration { get => moveDuration; }

        public int GetSlotIndex(Transform slotTransform)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] == slotTransform)
                    return i;
            }

            return -1;
        }

        public Transform GetSlotTransform(int index)
        {
            return index >= 0 && index < slots.Count ? slots[index] : null;
        }

        public bool TryGetEnemyAtSlot(int slotIndex, out EnemyView enemyView)
        {
            Transform slot = GetSlotTransform(slotIndex);
            if (slot == null)
            {
                enemyView = null;
                return false;
            }

            foreach (EnemyView ev in EnemyViews)
            {
                if (ev != null && ev.transform.parent == slot)
                {
                    enemyView = ev;
                    return true;
                }
            }

            enemyView = null;
            return false;
        }

        public void ShiftEnemies(int oldPlayerIndex, int newPlayerIndex, Action<int, int> onEnemyShifted = null, Action onComplete = null)
        {
            Sequence seq = DOTween.Sequence();
            int dir = newPlayerIndex > oldPlayerIndex ? -1 : 1;
            int start = Mathf.Min(oldPlayerIndex, newPlayerIndex);
            int end = Mathf.Max(oldPlayerIndex, newPlayerIndex);

            for (int i = start; i <= end; i++)
            {
                EnemyView enemy = GetEnemyAtSlot(i);
                if (enemy != null)
                {
                    Transform targetSlot = slots[i + dir];
                    onEnemyShifted?.Invoke(i, i + dir);
                    seq.Join(enemy.transform
                        .DOMove(targetSlot.position, moveDuration)
                        .SetEase(Ease.OutCubic)
                        .OnComplete(() => enemy.transform.SetParent(targetSlot)));
                }
            }

            if (onComplete != null)
                seq.OnComplete(onComplete.Invoke);
        }

        public Transform GetFirstAvailableSlot()
        {
            if (EnemyViews.Count >= maxEnemyCount)
                return null;

            foreach (int index in mPriorityQueue)
            {
                if (index < slots.Count && !IsSlotOccupied(slots[index]))
                    return slots[index];
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

        private EnemyView GetEnemyAtSlot(int slotIndex)
        {
            Transform slot = GetSlotTransform(slotIndex);
            if (slot == null) return null;

            foreach (EnemyView ev in EnemyViews)
            {
                if (ev != null && ev.transform.parent == slot)
                    return ev;
            }

            return null;
        }

        public EnemyView SpawnEnemy(Transform slot)
        {
            EnemyView enemyView = Instantiate(enemyPrefab, slot.position, slot.rotation, slot);
            EnemyViews.Add(enemyView);
            return enemyView;
        }

        public void RemoveEnemy(EnemyView enemyView)
        {
            EnemyViews.Remove(enemyView);
            Destroy(enemyView.gameObject);
        }
    }
}