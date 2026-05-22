using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    public class GameObjectPool : Pool<GameObject>
    {
        public GameObject Prefab { get; private set; }

        public void Init(GameObject prefab, int maxCount, int initCount = 0)
        {
            Prefab = prefab;
            mMaxCount = maxCount;
            SetFactoryMethod(() => Object.Instantiate(prefab));

            for (int i = 0; i < initCount; i++)
            {
                GameObject go = mFactory.Create();
                go.SetActive(false);
                mCacheStack.Push(go);
            }
        }

        public GameObject Get(Transform parent = null)
        {
            GameObject go = Allocate();
            go.SetActive(true);
            if (parent != null)
            {
                go.transform.SetParent(parent);
            }

            return go;
        }

        public override bool Recycle(GameObject obj)
        {
            if (obj == null) return false;

            if (mMaxCount > 0 && mCacheStack.Count >= mMaxCount)
            {
                Object.Destroy(obj);
                return false;
            }

            obj.SetActive(false);
            obj.transform.SetParent(null);
            mCacheStack.Push(obj);
            return true;
        }
    }
}