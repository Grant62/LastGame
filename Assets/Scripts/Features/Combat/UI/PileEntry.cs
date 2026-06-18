using System.Collections.Generic;
using Core.Architecture;
using Features.Card.Data;
using Features.Card.Model;
using Features.Card.UI;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Combat.UI
{
    public partial class PileEntry : MonoBehaviour, IController
    {
        [BoxGroup("牌堆配置")]
        [SerializeField] private PileType pileType;

        private ICardModel mCardModel;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            mCardModel = this.GetModel<ICardModel>();

            switch (pileType)
            {
                case PileType.DrawPile:
                    mCardModel.OnDrawPileChanged.Register(RefreshCount);
                    break;
                case PileType.DiscardPile:
                    mCardModel.OnDiscardPileChanged.Register(RefreshCount);
                    break;
                case PileType.Library:
                    mCardModel.OnLibraryChanged.Register(RefreshCount);
                    break;
                case PileType.ConsumePile:
                    mCardModel.OnConsumePileChanged.Register(RefreshCount);
                    break;
            }

            RefreshCount();

            Button btn = GetComponent<Button>();
            btn.onClick.AddListener(OnClick);
        }

        private void RefreshCount()
        {
            int count = pileType switch
            {
                PileType.DrawPile => mCardModel.DrawPile.Count,
                PileType.DiscardPile => mCardModel.DiscardPile.Count,
                PileType.Library => mCardModel.Library.Count,
                PileType.ConsumePile => mCardModel.ConsumePile.Count,
                _ => 0
            };

            CountLabel.text = count.ToString();

            if (pileType == PileType.ConsumePile)
                gameObject.SetActive(count > 0);
        }

        private void OnClick()
        {
            List<CardData> pile = pileType switch
            {
                PileType.DrawPile => mCardModel.DrawPile,
                PileType.DiscardPile => mCardModel.DiscardPile,
                PileType.Library => mCardModel.Library,
                PileType.ConsumePile => mCardModel.ConsumePile,
                _ => null
            };

            if (pile != null)
                UIKit.OpenPanel<PileGridPanel>(UILevel.PopUI, new PileGridPanelData { Cards = pile });
        }

        private void OnDestroy()
        {
            switch (pileType)
            {
                case PileType.DrawPile:
                    mCardModel.OnDrawPileChanged.UnRegister(RefreshCount);
                    break;
                case PileType.DiscardPile:
                    mCardModel.OnDiscardPileChanged.UnRegister(RefreshCount);
                    break;
                case PileType.Library:
                    mCardModel.OnLibraryChanged.UnRegister(RefreshCount);
                    break;
                case PileType.ConsumePile:
                    mCardModel.OnConsumePileChanged.UnRegister(RefreshCount);
                    break;
            }
        }
    }
}