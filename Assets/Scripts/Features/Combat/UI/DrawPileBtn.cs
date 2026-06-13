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
        [SerializeField] private PileGridPanel pileGrid;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            mCardModel = this.GetModel<ICardModel>();

            Button btn = GetComponent<Button>();
            btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            pileGrid.Open(mCardModel.Library);
        }
    }
}