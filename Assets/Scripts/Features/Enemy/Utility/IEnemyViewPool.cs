using Features.Enemy.View;
using QFramework;
using UnityEngine;

namespace Features.Enemy.Utility
{
    public interface IEnemyViewPool : IUtility
    {
        EnemyView Get(Transform parent);
        void Return(EnemyView enemy);
        void Dispose();
    }
}