using System;
using System.Collections.Generic;
using Core.Architecture;
using DG.Tweening;
using Features.Combat.Event;
using Features.Enemy.Model;
using Features.Enemy.Utility;
using Features.Enemy.View;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Combat.View.Board
{
    public class BoardView : MonoBehaviour, IController
    {
        [BoxGroup("引用")]
        [SerializeField] private RectTransform boardPanel;

        public RectTransform BoardPanel { get => boardPanel; }

        [BoxGroup("引用")]
        [SerializeField] private SlotView slotPrefab;
        [BoxGroup("引用")]
        [SerializeField] private EnemyView enemyPrefab;

        public EnemyView EnemyPrefab { get => enemyPrefab; }

        [BoxGroup("棋盘布局")]
        [SerializeField] private int slotCount = 9;
        [BoxGroup("棋盘布局")]
        [SerializeField] private float slotSpacing = 2f;

        [BoxGroup("动画")]
        [SerializeField] private float moveDuration = 0.15f;

        private readonly List<SlotView> mSlots = new();
        public List<EnemyView> EnemyViews { get; } = new();

        public int SlotCount { get => mSlots.Count; }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            CreateSlots();
            this.RegisterEvent<EnemyDiedEvent>(OnEnemyDied).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnEnemyDied(EnemyDiedEvent @event)
        {
            IEnemyModel enemyModel = this.GetModel<IEnemyModel>();
            enemyModel.RemoveEnemy(@event.SlotIndex);

            for (int i = EnemyViews.Count - 1; i >= 0; i--)
            {
                if (EnemyViews[i].SlotIndex == @event.SlotIndex)
                {
                    RemoveEnemy(EnemyViews[i]);
                    break;
                }
            }
        }

        public void ClearAllEnemies()
        {
            for (int i = EnemyViews.Count - 1; i >= 0; i--)
            {
                EnemyViews[i].gameObject.SetActive(false);
                RemoveEnemy(EnemyViews[i]);
            }

            EnemyViews.Clear();
        }

        public IEnumerable<EnemyView> GetActiveEnemies()
        {
            foreach (EnemyView enemy in EnemyViews)
            {
                if (enemy.isActiveAndEnabled)
                    yield return enemy;
            }
        }

        private void CreateSlots()
        {
            for (int i = 0; i < slotCount; i++)
            {
                SlotView slot = Instantiate(slotPrefab, boardPanel);
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

        public EnemyView GetEnemyAtSlot(int slotIndex)
        {
            foreach (EnemyView enemy in GetActiveEnemies())
                if (enemy.SlotIndex == slotIndex)
                    return enemy;
            return null;
        }

        public bool TryGetEnemyAtSlot(int slotIndex, out EnemyView enemy)
        {
            enemy = GetEnemyAtSlot(slotIndex);
            return enemy != null;
        }

        public void ForEachSlotOnPath(int fromSlot, int toSlot, Action<int> onStep)
        {
            int step = toSlot > fromSlot ? 1 : -1;
            for (int i = fromSlot; i != toSlot + step; i += step)
                onStep(i);
        }

        public EnemyView SpawnEnemy(int slotIndex)
        {
            SlotView slot = mSlots[slotIndex];
            EnemyView enemy = this.GetUtility<IEnemyViewPool>().Get(slot.transform);
            enemy.SlotIndex = slotIndex;
            EnemyViews.Add(enemy);
            return enemy;
        }

        public void RemoveEnemy(EnemyView enemy)
        {
            EnemyViews.Remove(enemy);
            this.GetUtility<IEnemyViewPool>().Return(enemy);
        }

        public int FindLeftEmptySlot(int heroSlot)
        {
            for (int i = 0; i < heroSlot; i++)
            {
                if (GetEnemyAtSlot(i) == null)
                    return i;
            }

            return -1;
        }

        public int FindRightEmptySlot(int heroSlot)
        {
            for (int i = 8; i > heroSlot; i--)
            {
                if (GetEnemyAtSlot(i) == null)
                    return i;
            }

            return -1;
        }

        public void MoveEnemy(int fromSlot, int toSlot)
        {
            EnemyView enemy = GetEnemyAtSlot(fromSlot);
            if (enemy == null)
                return;

            SlotView targetSlot = mSlots[toSlot];
            enemy.SlotIndex = toSlot;
            enemy.transform.SetParent(targetSlot.SlotRect, false);
            enemy.transform.DOMove(targetSlot.SlotRect.position, moveDuration).SetEase(Ease.OutCubic);
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

            EnemyView enemy = GetEnemyAtSlot(from);
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