using System.Collections.Generic;
using Core.Architecture;
using Core.SceneManagement;
using Core.SceneManagement.Define;
using Core.Systems;
using Cysharp.Threading.Tasks;
using Features.Card.Data;
using Features.Card.Model;
using Features.Card.UI;
using Features.Card.Utility;
using Features.Card.View;
using Features.Resource.Command;
using Features.Resource.Model;
using Features.Shop.Command;
using Features.Shop.Data;
using Features.Shop.Model;
using Features.Shop.System;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Features.Shop.View
{
    public class ShopRoom : SceneBase, IController
    {
        public override string SceneId { get => "ShopRoomRoot"; }

        public override SceneContainerType ContainerType { get => SceneContainerType.Room; }

        [SerializeField] private ShopItemSlot[] cardPackSlots;
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private Button leaveButton;
        [SerializeField] private CardView cardViewPrefab;
        [SerializeField] private CardPickPanel cardPickPanel;
        [SerializeField] private Button removeCardButton;
        [SerializeField] private TMP_Text removePriceText;

        public new IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            leaveButton.onClick.AddListener(OnLeave);
            removeCardButton.onClick.AddListener(OnRemoveCard);
        }

        public override UniTask OnSceneEnter(SceneLoadContext ctx)
        {
            GameMain.Interface.RegisterUtility<ICardViewPool>(new CardViewPool(cardViewPrefab, transform));

            this.GetSystem<IShopSystem>().GenerateShop();
            RenderAll();

            IResourceModel resourceModel = this.GetModel<IResourceModel>();
            resourceModel.Gold.Register(_ => UpdateGoldDisplay())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            IShopModel shopModel = this.GetModel<IShopModel>();
            shopModel.OnShopChanged.Register(RenderAll)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            return UniTask.CompletedTask;
        }

        public override UniTask OnSceneExit()
        {
            this.GetUtility<ICardViewPool>().Dispose();
            return UniTask.CompletedTask;
        }

        private void RenderAll()
        {
            IShopModel shopModel = this.GetModel<IShopModel>();
            UpdateGoldDisplay();
            UpdateRemovePrice();

            List<ShopCardPackSlot> slots = shopModel.CardPackSlots;
            for (int i = 0; i < cardPackSlots.Length && i < slots.Count; i++)
            {
                if (cardPackSlots[i] == null)
                    continue;

                int capturedIndex = i;
                ShopCardPackSlot slotData = slots[i];
                Sprite sprite = string.IsNullOrEmpty(slotData.Address)
                    ? null
                    : this.GetUtility<ICardSpriteCache>().GetSprite(slotData.Address);
                cardPackSlots[i].Render(slotData, sprite, () => OnBuyCardPack(capturedIndex));
            }
        }

        private void UpdateGoldDisplay()
        {
            goldText.text = $"{this.GetModel<IResourceModel>().Gold.Value}金";
        }

        private void OnBuyCardPack(int slotIndex)
        {
            IShopModel shopModel = this.GetModel<IShopModel>();
            ShopCardPackSlot slot = shopModel.CardPackSlots[slotIndex];

            if (slot.IsSold)
                return;

            IResourceModel resourceModel = this.GetModel<IResourceModel>();
            if (resourceModel.Gold.Value < slot.Price)
                return;

            this.SendCommand(new SpendGoldCommand(slot.Price));

            List<CardData> candidates = this.GetSystem<IShopSystem>().GenerateCandidates(slotIndex);
            OpenCardPickPanel(candidates, slotIndex);
        }

        private void OnRemoveCard()
        {
            OnRemoveCardAsync().Forget();
        }

        private async UniTask OnRemoveCardAsync()
        {
            IShopModel shopModel = this.GetModel<IShopModel>();
            int price = shopModel.CurrentRemovePrice;

            IResourceModel resourceModel = this.GetModel<IResourceModel>();
            if (resourceModel.Gold.Value < price)
                return;

            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(
                "RemoveCardPanel", GameRoot.PopUILayer);
            GameObject instance = await handle.Task;
            RemoveCardPanel panel = instance.GetComponent<RemoveCardPanel>();
            panel.Show(
                this.GetModel<ICardModel>().Library,
                card =>
                {
                    this.SendCommand(new SpendGoldCommand(price));
                    this.SendCommand(new RemoveCardFromLibraryCommand(card));
                });
        }

        private void UpdateRemovePrice()
        {
            int price = this.GetModel<IShopModel>().CurrentRemovePrice;
            removePriceText.text = $"{price}金";
        }

        private void OpenCardPickPanel(List<CardData> candidates, int slotIndex)
        {
            ShopCardPackSlot slot = this.GetModel<IShopModel>().CardPackSlots[slotIndex];

            cardPickPanel.Show(new CardPickPanelData
            {
                Candidates = candidates,
                PickCount = slot.PickCount,
                PackPrice = slot.Price,
                OnConfirmed = selectedCards => { this.SendCommand(new BuyCardPackCommand(slotIndex, selectedCards)); },
                OnCancelled = () => { this.SendCommand(new AddGoldCommand(slot.Price)); }
            });
        }

        private void OnLeave()
        {
            this.GetSystem<ISceneManager>().LoadRoomScene("PreBattleRoomRoot").Forget();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                PileGridPanel.ToggleDrawPile(this.GetModel<ICardModel>().Library);

            if (Input.GetKeyDown(KeyCode.Escape))
                this.GetSystem<IPopupStackSystem>().HandleEsc();
        }
    }
}