using Core.Architecture;
using QFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Main.GM
{
    public class GmSystem : AbstractSystem
    {
        private GmPanel mPanel;
        private AsyncOperationHandle<GameObject> mHandle;

        protected override void OnInit()
        {
            mHandle = Addressables.LoadAssetAsync<GameObject>("GmPanel");
            ActionKit.OnUpdate.Register(OnUpdate);
        }

        private void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.BackQuote))
                return;

            if (mPanel == null)
            {
                if (!mHandle.IsDone)
                    return;

                GameObject instance = Object.Instantiate(mHandle.Result, GameRoot.PopUILayer);
                instance.transform.SetAsLastSibling();
                mPanel = instance.GetComponent<GmPanel>();
                mPanel.Close();
            }

            if (mPanel.gameObject.activeSelf)
                mPanel.Close();
            else
            {
                mPanel.gameObject.SetActive(true);
                mPanel.Open();
            }
        }
    }
}