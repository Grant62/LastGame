using Core.Architecture;
using Features.Card.Model;
using Features.Card.UI;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Combat.UI
{
    public partial class DrawPileBtn : ViewController, IController
    {
        private ICardModel mCardModel;
        private PileGridPanel mPileGrid;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            mCardModel = this.GetModel<ICardModel>();
            mPileGrid = FindFirstObjectByType<PileGridPanel>(FindObjectsInactive.Include);

            Button btn = GetComponent<Button>();
            btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (mPileGrid != null)
                mPileGrid.Open(mCardModel.Library);
        }
    }
}