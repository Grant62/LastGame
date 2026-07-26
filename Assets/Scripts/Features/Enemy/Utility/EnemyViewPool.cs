using DG.Tweening;
using Features.Enemy.View;
using QFramework;
using UnityEngine;

namespace Features.Enemy.Utility
{
    public class EnemyViewPool : IEnemyViewPool
    {
        private readonly SimpleObjectPool<EnemyView> mPool;
        private readonly Transform mPoolRoot;
        private EnemyView mPrefab;
        private bool mDisposed;

        public EnemyViewPool(EnemyView prefab)
        {
            mPrefab = prefab;
            GameObject go = new("[Pool] EnemyView");
            go.SetActive(false);
            mPoolRoot = go.transform;

            mPool = new SimpleObjectPool<EnemyView>(
                () =>
                {
                    EnemyView obj = Object.Instantiate(mPrefab, mPoolRoot);
                    obj.gameObject.SetActive(false);
                    return obj;
                },
                null,
                5
            );
        }

        public EnemyView Get(Transform parent)
        {
            if (mDisposed || mPrefab == null)
                return null;

            EnemyView enemy = mPool.Allocate();
            if (enemy == null || !enemy)
                return null;

            enemy.transform.SetParent(parent, false);
            enemy.transform.localScale = Vector3.one;
            enemy.transform.DOKill();
            enemy.gameObject.SetActive(true);
            return enemy;
        }

        public void Return(EnemyView enemy)
        {
            if (mDisposed || enemy == null || !enemy)
                return;

            enemy.gameObject.SetActive(false);
            enemy.transform.SetParent(mPoolRoot, false);
            mPool.Recycle(enemy);
        }

        public void Dispose()
        {
            mDisposed = true;
            mPrefab = null;
            if (mPoolRoot != null)
                Object.Destroy(mPoolRoot.gameObject);
        }
    }
}
