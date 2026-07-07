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

        public EnemyViewPool(EnemyView prefab)
        {
            GameObject go = new("[Pool] EnemyView");
            go.SetActive(false);
            mPoolRoot = go.transform;

            mPool = new SimpleObjectPool<EnemyView>(
                () =>
                {
                    EnemyView obj = Object.Instantiate(prefab, mPoolRoot);
                    obj.gameObject.SetActive(false);
                    return obj;
                },
                null,
                5
            );
        }

        public EnemyView Get(Transform parent)
        {
            EnemyView enemy = mPool.Allocate();
            enemy.transform.SetParent(parent, false);
            enemy.transform.localScale = Vector3.one;
            enemy.transform.DOKill();
            enemy.gameObject.SetActive(true);
            return enemy;
        }

        public void Return(EnemyView enemy)
        {
            enemy.gameObject.SetActive(false);
            enemy.transform.SetParent(mPoolRoot, false);
            mPool.Recycle(enemy);
        }

        public void Dispose()
        {
            Object.Destroy(mPoolRoot.gameObject);
        }
    }
}