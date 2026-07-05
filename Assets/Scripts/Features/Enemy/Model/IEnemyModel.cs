using System.Collections.Generic;
using Features.Enemy.Data;
using Features.Enemy.Define;
using QFramework;

namespace Features.Enemy.Model
{
    public interface IEnemyModel : IModel
    {
        List<EnemyRuntimeData> ActiveEnemies { get; }

        EasyEvent OnEnemiesChanged { get; }

        int[] EnemyIdQueue { get; set; }

        int QueueIndex { get; set; }

        Dictionary<int, EnemyIntentType> CachedIntents { get; }

        void AddEnemy(EnemyRuntimeData data);

        void RemoveEnemy(int slotIndex);

        EnemyRuntimeData GetEnemyAtSlot(int slotIndex);

        bool AnyAlive { get; }

        bool HasMoreInQueue { get; }

        bool IsStepComplete { get; }

        void ClearAll();
    }
}