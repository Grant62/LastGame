using Features.Enemy.View;
using QFramework;
using UnityEngine;

namespace Features.Enemy.Utility
{
    public class EnemyViewPool : IEnemyViewPool
    {
        private readonly SimpleObjectPool<EnemyView> mPool;
        private readonly Transform mPoolRoot;

        public EnemyViewPool(EnemyView prefab)
        {
            mPool = new SimpleObjectPool<EnemyView>(
                () => Object.Instantiate(prefab),
                null,
                5
            );

            GameObject go = new("[Pool] EnemyView");
            go.SetActive(false);
            mPoolRoot = go.transform;
        }

        public EnemyView Get(Transform parent)
        {
            EnemyView enemy = mPool.Allocate();
            enemy.transform.SetParent(parent, false);
            enemy.transform.localScale = Vector3.one;
            enemy.gameObject.SetActive(true);
            return enemy;
        }

        public void Return(EnemyView enemy)
        {
            enemy.transform.SetParent(mPoolRoot, false);
            mPool.Recycle(enemy);
        }
    }
}