using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public static class GameObjectPoolSystem
    {
        private static Dictionary<string, GameObjectPool> mPools;
        private static Transform mPoolRoot;

        private static void EnsureInitialized()
        {
            if (mPools != null) return;
            mPools = new Dictionary<string, GameObjectPool>();
            mPoolRoot = new GameObject("GameObjectPoolRoot").transform;
            mPoolRoot.position = Vector3.zero;
            Object.DontDestroyOnLoad(mPoolRoot.gameObject);
        }

        public static void Init(string key, GameObject prefab, int maxCount = -1, int initCount = 0)
        {
            EnsureInitialized();

            if (mPools.TryGetValue(key, out GameObjectPool existing))
            {
                existing.Init(prefab, maxCount, initCount);
            }
            else
            {
                var pool = new GameObjectPool();
                pool.Init(prefab, maxCount, initCount);
                mPools[key] = pool;
            }
        }

        public static GameObject Get(string key, Transform parent = null)
        {
            EnsureInitialized();

            if (mPools.TryGetValue(key, out GameObjectPool pool))
            {
                GameObject go = pool.Get(parent);
                if (parent == null)
                {
                    go.transform.SetParent(null);
                }
                return go;
            }
            return null;
        }

        public static T Get<T>(string key, Transform parent = null) where T : Component
        {
            GameObject go = Get(key, parent);
            return go != null ? go.GetComponent<T>() : null;
        }

        public static bool Push(string key, GameObject obj)
        {
            EnsureInitialized();

            if (mPools.TryGetValue(key, out GameObjectPool pool))
            {
                obj.transform.SetParent(mPoolRoot);
                return pool.Recycle(obj);
            }

            var newPool = new GameObjectPool();
            newPool.Init(null, -1);
            obj.transform.SetParent(mPoolRoot);
            newPool.Recycle(obj);
            mPools[key] = newPool;
            return true;
        }

        public static void Clear(string key)
        {
            if (mPools == null) return;

            if (mPools.TryGetValue(key, out GameObjectPool pool))
            {
                pool.Clear(Object.Destroy);
                mPools.Remove(key);
            }
        }

        public static void ClearAll()
        {
            if (mPools == null) return;

            foreach (GameObjectPool pool in mPools.Values)
            {
                pool.Clear(Object.Destroy);
            }
            mPools.Clear();
        }

        public static int Count(string key)
        {
            if (mPools == null) return 0;
            return mPools.TryGetValue(key, out GameObjectPool pool) ? pool.CurCount : 0;
        }
    }
}
