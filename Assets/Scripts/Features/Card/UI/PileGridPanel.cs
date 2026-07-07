using System.Collections.Generic;
using Core.Architecture;
using Features.Card.Data;
using Features.Card.Interfaces;
using Features.Card.View;
using Features.Combat.System;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Features.Card.UI
{
    public partial class PileGridPanel : MonoBehaviour, IController
    {
        private readonly List<CardView> mCardViews = new();
        private static PileGridPanel sInstance;

        [BoxGroup("滚动")]
        [SerializeField] private float scrollSensitivity = 30f;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            GetComponent<ScrollRect>().scrollSensitivity = scrollSensitivity;
            Close.onClick.AddListener(OnClose);
        }

        public static async void ShowOrCreate(List<CardData> cards)
        {
            if (sInstance == null)
            {
                AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(
                    "PileGridPanel", GameRoot.PopUILayer);
                GameObject instance = await handle.Task;
                sInstance = instance.GetComponent<PileGridPanel>();
            }

            sInstance.Show(cards);
        }

        public static async void ToggleDrawPile(List<CardData> cards)
        {
            if (sInstance != null && sInstance.gameObject.activeSelf)
            {
                sInstance.OnClose();
                return;
            }

            ShowOrCreate(cards);
        }

        private void Show(List<CardData> cards)
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            LayoutCards(cards);
            GameMain.Interface.GetSystem<IPopupStackSystem>().Push(gameObject);
        }

        private void OnClose()
        {
            ReleaseCards();
            gameObject.SetActive(false);
            GameMain.Interface.GetSystem<IPopupStackSystem>().Remove(gameObject);
        }

        private void OnDestroy()
        {
            if (sInstance == this)
                sInstance = null;
        }

        private void LayoutCards(List<CardData> cards)
        {
            ReleaseCards();

            foreach (CardData cardData in cards)
            {
                CardView card = this.GetUtility<ICardViewPool>().Get(cardData, Content, false);
                card.HandDragHandler.enabled = false;
                card.CardHoverHandler.enabled = false;
                card.RectTransform.localEulerAngles = Vector3.zero;
                mCardViews.Add(card);
            }
        }

        private void ReleaseCards()
        {
            ICardViewPool pool = this.GetUtility<ICardViewPool>();
            foreach (CardView view in mCardViews)
                pool.Return(view);
            mCardViews.Clear();
        }
    }
}