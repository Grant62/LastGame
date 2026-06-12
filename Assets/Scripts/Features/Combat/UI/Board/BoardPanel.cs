using System.Collections.Generic;
using Core.Architecture;
using DG.Tweening;
using Features.Combat.Event;
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
            this.RegisterEvent<EnemyDiedEvent>(OnEnemyDied).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            for (int i = EnemyViews.Count - 1; i >= 0; i--)
            {
                if (EnemyViews[i].SlotIndex == e.SlotIndex)
                {
                    EnemyViews.RemoveAt(i);
                    break;
                }
            }
        }

        public IEnumerable<EnemyUI> GetActiveEnemies()
        {
            foreach (EnemyUI enemy in EnemyViews)
            {
                if (enemy.isActiveAndEnabled)
                    yield return enemy;
            }
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
            foreach (EnemyUI enemy in GetActiveEnemies())
                if (enemy.SlotIndex == slotIndex)
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

        public void ShiftEnemies(int oldPlayerIndex, int newPlayerIndex)
        {
            Sequence seq = DOTween.Sequence();
            bool movingRight = newPlayerIndex > oldPlayerIndex;
            int pushDir = movingRight ? -1 : 1;

            if (movingRight)
            {
                for (int i = oldPlayerIndex; i <= newPlayerIndex; i++)
                    TryShiftEnemy(i, i + pushDir, seq);
            }
            else
            {
                for (int i = oldPlayerIndex; i >= newPlayerIndex; i--)
                    TryShiftEnemy(i, i + pushDir, seq);
            }
        }

        private void TryShiftEnemy(int from, int to, Sequence seq)
        {
            if (to < 0 || to >= mSlots.Count)
                return;

            EnemyUI enemy = GetEnemyAtSlot(from);
            if (enemy == null)
                return;

            enemy.SlotIndex = to;
            enemy.transform.SetParent(mSlots[to].SlotRect, false);
            seq.Join(enemy.transform
                .DOMove(mSlots[to].SlotRect.position, moveDuration)
                .SetEase(Ease.OutCubic));
        }
    }
}