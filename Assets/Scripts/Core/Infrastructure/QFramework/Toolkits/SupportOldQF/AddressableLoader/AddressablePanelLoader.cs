using System;
using QFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Infrastructure.QFramework.Toolkits.SupportOldQF
{
    public class AddressablePanelLoaderPool : AbstractPanelLoaderPool
    {
        public class AddressablePanelLoader : IPanelLoader
        {
            private AsyncOperationHandle<GameObject> mHandle;

            public GameObject LoadPanelPrefab(PanelSearchKeys panelSearchKeys)
            {
                string address = panelSearchKeys.GameObjName.IsNotNullAndEmpty()
                    ? panelSearchKeys.GameObjName
                    : panelSearchKeys.PanelType?.Name;

                if (string.IsNullOrEmpty(address))
                    throw new InvalidOperationException(
                        $"AddressablePanelLoader: no address for panel type {panelSearchKeys.PanelType}");

                mHandle = Addressables.LoadAssetAsync<GameObject>(address);
                mHandle.WaitForCompletion();
                if (mHandle.Result == null)
                    throw new InvalidOperationException($"Addressables load failed: [{address}]");
                return mHandle.Result;
            }

            public void LoadPanelPrefabAsync(PanelSearchKeys panelSearchKeys, Action<GameObject> onPanelPrefabLoad)
            {
                string address = panelSearchKeys.GameObjName.IsNotNullAndEmpty()
                    ? panelSearchKeys.GameObjName
                    : panelSearchKeys.PanelType?.Name;

                if (string.IsNullOrEmpty(address))
                    throw new InvalidOperationException(
                        $"AddressablePanelLoader: no address for panel type {panelSearchKeys.PanelType}");

                mHandle = Addressables.LoadAssetAsync<GameObject>(address);
                mHandle.Completed += handle =>
                {
                    if (handle.Result == null)
                        throw new InvalidOperationException($"Addressables load failed: [{address}]");
                    onPanelPrefabLoad?.Invoke(handle.Result);
                };
            }

            public void Unload()
            {
                if (mHandle.IsValid())
                    Addressables.Release(mHandle);
            }
        }

        protected override IPanelLoader CreatePanelLoader()
        {
            return new AddressablePanelLoader();
        }
    }
}