using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Core.Infrastructure.QFramework.Toolkits.SupportOldQF.UIKitCreateService
{
    public class ResKitUIPanelTester : MonoBehaviour
    {
        /// <summary>
        ///     页面的名字
        /// </summary>
        public string PanelName;

        /// <summary>
        ///     层级名字
        /// </summary>
        public UILevel Level;

        [SerializeField] private List<UIPanelTesterInfo> mOtherPanels;

        private void Awake()
        {
            ResKit.Init();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.2f);

            UIKit.OpenPanel(PanelName, Level);

            mOtherPanels.ForEach(panelTesterInfo => { UIKit.OpenPanel(panelTesterInfo.PanelName, panelTesterInfo.Level); });
        }
    }
}