using System;
using System.Collections.Generic;
using Core.Architecture;
using DG.Tweening;
using QFramework;
using UnityEngine;

namespace Features.Combat.UI.Board
{
    public class BoardPanel : ViewController, IController
    {
        [SerializeField] private RectTransform boardPanel;
        [SerializeField] private SlotUI slotPrefab;
        [SerializeField] private EnemyUI enemyPrefab;
        [SerializeField] private int slotCount = 9;
        [SerializeField] private float slotSpacing = 2f;
        [SerializeField] private float moveDuration = 0.15f;

        private readonly List<SlotUI> mSlots = new();
        public List<EnemyUI> EnemyViews { get; } = new();

        public int SlotCount { get => mSlots.Count; }

        public float MoveDuration { get => moveDuration; }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            CreateSlots();
        }

        private void CreateSlots()
        {
            for (int i = 0; i < slotCount; i++)
            {
                SlotUI slot = Instantiate(slotPrefab, boardPanel);
                slot.SlotIndex = i;
                slot.SlotRect.anchoredPosition = new Vector2(
                    (i - (slotCount - 1) / 2f) * slotSpacing, 0);
                mSlots.Add(slot);
            }
        }

        public RectTransform GetSlotTransform(int index)
        {
            return index >= 0 && index < mSlots.Count ? mSlots[index].SlotRect : null;
        }

        public int GetSlotIndex(RectTransform slotRect)
        {
            for (int i = 0; i < mSlots.Count; i++)
                if (mSlots[i].SlotRect == slotRect)
                    return i;
            return -1;
        }

        public EnemyUI GetEnemyAtSlot(int slotIndex)
        {
            foreach (EnemyUI enemy in EnemyViews)
                if (enemy != null && enemy.SlotIndex == slotIndex)
                    return enemy;
            return null;
        }

        public bool TryGetEnemyAtSlot(int slotIndex, out EnemyUI enemy)
        {
            enemy = GetEnemyAtSlot(slotIndex);
            return enemy != null;
        }

        public EnemyUI SpawnEnemy(int slotIndex)
        {
            SlotUI slot = mSlots[slotIndex];
            EnemyUI enemy = Instantiate(enemyPrefab, slot.transform);
            enemy.SlotIndex = slotIndex;
            EnemyViews.Add(enemy);
            return enemy;
        }

        public void RemoveEnemy(EnemyUI enemy)
        {
            EnemyViews.Remove(enemy);
            Destroy(enemy.gameObject);
        }

        public void ShiftEnemies(int oldPlayerIndex, int newPlayerIndex,
            Action<int, int> onEnemyShifted = null, Action onComplete = null)
        {
            Sequence seq = DOTween.Sequence();
            int dir = newPlayerIndex > oldPlayerIndex ? -1 : 1;
            int start = Mathf.Min(oldPlayerIndex, newPlayerIndex);
            int end = Mathf.Max(oldPlayerIndex, newPlayerIndex);

            for (int i = start; i <= end; i++)
            {
                EnemyUI enemy = GetEnemyAtSlot(i);
                if (enemy != null)
                {
                    int targetIndex = i + dir;
                    if (targetIndex < 0 || targetIndex >= mSlots.Count)
                        continue;

                    RectTransform targetSlot = mSlots[targetIndex].SlotRect;
                    int from = i;
                    int to = targetIndex;
                    onEnemyShifted?.Invoke(from, to);
                    enemy.SlotIndex = to;
                    seq.Join(enemy.transform
                        .DOMove(targetSlot.position, moveDuration)
                        .SetEase(Ease.OutCubic));
                }
            }

            if (onComplete != null)
                seq.OnComplete(onComplete.Invoke);
        }
    }
}