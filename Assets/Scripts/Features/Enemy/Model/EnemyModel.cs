using System;
using System.Collections.Generic;
using Features.Enemy.Data;
using Features.Enemy.Define;
using QFramework;

namespace Features.Enemy.Model
{
    public class EnemyModel : AbstractModel, IEnemyModel
    {
        public List<EnemyRuntimeData> ActiveEnemies { get; } = new();

        public EasyEvent OnEnemiesChanged { get; } = new();

        public int[] EnemyIdQueue { get; set; } = Array.Empty<int>();

        public int QueueIndex { get; set; }

        public Dictionary<int, EnemyIntentType> CachedIntents { get; } = new();

        protected override void OnInit() { }

        public bool HasMoreInQueue
        {
            get => QueueIndex < EnemyIdQueue.Length;
        }

        public bool IsStepComplete
        {
            get => !HasMoreInQueue && !AnyAlive;
        }

        public void AddEnemy(EnemyRuntimeData data)
        {
            ActiveEnemies.Add(data);
            OnEnemiesChanged.Trigger();
        }

        public void RemoveEnemy(int slotIndex)
        {
            for (int i = ActiveEnemies.Count - 1; i >= 0; i--)
            {
                if (ActiveEnemies[i].SlotIndex == slotIndex)
                {
                    ActiveEnemies.RemoveAt(i);
                    OnEnemiesChanged.Trigger();
                    return;
                }
            }
        }

        public EnemyRuntimeData GetEnemyAtSlot(int slotIndex)
        {
            for (int i = 0; i < ActiveEnemies.Count; i++)
            {
                if (ActiveEnemies[i].SlotIndex == slotIndex)
                    return ActiveEnemies[i];
            }

            return null;
        }

        public bool AnyAlive
        {
            get
            {
                for (int i = 0; i < ActiveEnemies.Count; i++)
                {
                    if (ActiveEnemies[i].HP > 0)
                        return true;
                }

                return false;
            }
        }

        public void ClearAll()
        {
            ActiveEnemies.Clear();
            CachedIntents.Clear();
            EnemyIdQueue = Array.Empty<int>();
            QueueIndex = 0;
            OnEnemiesChanged.Trigger();
        }
    }
}