#if ENABLE_ADDRESSABLES
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace QFramework
{
    public class AddressablePanelLoaderPool : AbstractPanelLoaderPool
    {
        protected override IPanelLoader CreatePanelLoader()
        {
            return new AddressablePanelLoader();
        }

        public class AddressablePanelLoader : IPanelLoader
        {
            private GameObject mPanelInstance;
            private string mPanelKey;

            public GameObject LoadPanelPrefab(PanelSearchKeys panelSearchKeys)
            {
                mPanelKey = panelSearchKeys.GameObjName ?? panelSearchKeys.PanelType.Name;

                GameObject pooled = GameObjectPoolSystem.Get(mPanelKey);
                if (pooled != null)
                {
                    mPanelInstance = pooled;
                    return pooled;
                }

                mPanelInstance = Addressables.InstantiateAsync(mPanelKey).WaitForCompletion();
                mPanelInstance.name = mPanelKey;
                return mPanelInstance;
            }

            public void LoadPanelPrefabAsync(PanelSearchKeys panelSearchKeys, Action<GameObject> onPanelPrefabLoad)
            {
                mPanelKey = panelSearchKeys.GameObjName ?? panelSearchKeys.PanelType.Name;

                GameObject pooled = GameObjectPoolSystem.Get(mPanelKey);
                if (pooled != null)
                {
                    mPanelInstance = pooled;
                    onPanelPrefabLoad?.Invoke(pooled);
                    return;
                }

                Addressables.InstantiateAsync(mPanelKey).Completed += handle =>
                {
                    handle.Result.name = mPanelKey;
                    mPanelInstance = handle.Result;
                    onPanelPrefabLoad?.Invoke(handle.Result);
                };
            }

            public void Unload()
            {
                if (mPanelInstance != null)
                {
                    GameObjectPoolSystem.Push(mPanelKey, mPanelInstance);
                    mPanelInstance = null;
                }
            }
        }
    }
}
#endif