using System.Collections.Generic;
using Core.Architecture;
using DG.Tweening;
using Features.Card.Command;
using Features.Card.Data;
using Features.Card.Model;
using Features.Combat.Interaction;
using Features.Combat.Targeting;
using Features.Combat.Targeting.Command;
using Features.Hero.Command;
using Features.Hero.Model;
using Features.Sword.Command;
using Features.Sword.System;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Card.UI
{
    public class HandDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IController
    {
        private Canvas mTargetCanvas;
        private ISlotTargetSystem mSlotSystem;
        private CardUI mCardUI;
        private GameObject mDragGhost;
        private bool mIsDragging;

        private readonly PointerEventData mCachedPed = new(EventSystem.current);
        private readonly List<RaycastResult> mCachedResults = new();

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            mCardUI = GetComponent<CardUI>();
            mTargetCanvas = GetComponentInParent<Canvas>();
            mSlotSystem = this.GetSystem<ISlotTargetSystem>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (mCardUI == null || mCardUI.CardData == null)
                return;
            if (!this.GetSystem<IInteractionSystem>().CanInteract())
                return;

            mIsDragging = true;
            this.GetSystem<IInteractionSystem>().BeginDrag();
            GetComponent<RectTransform>().DOKill();
            CreateDragGhost(eventData.position);

            if (IsTargetingCard())
                this.SendCommand(new StartTargetingCommand(transform.position));
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!mIsDragging || mDragGhost == null)
                return;

            mDragGhost.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!mIsDragging || mCardUI == null || mCardUI.CardData == null)
                return;

            mIsDragging = false;
            this.GetSystem<IInteractionSystem>().EndDrag();
            this.SendCommand<EndTargetingCommand>();

            bool played = false;

            if (IsEnemyTargetCard())
                played = PlayWithEnemyTarget(eventData.position);
            else if (IsSlotTargetCard())
                played = PlayWithSlotTarget(eventData.position);
            else
                played = PlayNormal();

            DestroyDragGhost();

            if (!played)
            {
                ICardModel model = this.GetModel<ICardModel>();
                if (model.HandPile.Contains(mCardUI.CardData))
                    return;
            }
        }

        private bool PlayWithEnemyTarget(Vector2 screenPos)
        {
            IEnemyTarget enemyTarget = RaycastFor<IEnemyTarget>(screenPos);
            if (enemyTarget == null || !enemyTarget.IsValidTarget)
                return false;

            SlotAction action = mCardUI.CardData.SlotAction;

            this.SendCommand(new PlayCardCommand(mCardUI.CardData, enemyTarget));

            if (enemyTarget.SlotIndex >= 0)
            {
                UpdateFacing(enemyTarget.SlotIndex);

                if (action == SlotAction.MoveSword)
                    this.SendCommand(new MoveSwordCommand(enemyTarget.SlotIndex));
                else if (action == SlotAction.MovePlayer)
                    this.SendCommand(new MovePlayerCommand(enemyTarget.SlotIndex));
            }

            return true;
        }

        private bool PlayWithSlotTarget(Vector2 screenPos)
        {
            ISlotTarget slotTarget = RaycastFor<ISlotTarget>(screenPos);
            if (slotTarget == null)
                return false;

            int slotIndex = slotTarget.SlotIndex;
            if (!mSlotSystem.Validate(mCardUI.CardData, slotIndex))
                return false;

            SlotAction action = mCardUI.CardData.SlotAction;

            this.SendCommand(new PlayCardCommand(mCardUI.CardData));
            UpdateFacing(slotIndex);

            if (action == SlotAction.MoveSword)
                this.SendCommand(new MoveSwordCommand(slotIndex));
            else if (action == SlotAction.MovePlayer)
                this.SendCommand(new MovePlayerCommand(slotIndex));

            return true;
        }

        private bool PlayNormal()
        {
            this.SendCommand(new PlayCardCommand(mCardUI.CardData));
            return true;
        }

        private T RaycastFor<T>(Vector2 screenPos) where T : class
        {
            mCachedPed.position = screenPos;
            mCachedResults.Clear();

            if (EventSystem.current != null)
                EventSystem.current.RaycastAll(mCachedPed, mCachedResults);

            foreach (RaycastResult result in mCachedResults)
            {
                T component = result.gameObject.GetComponentInParent<T>();
                if (component != null)
                    return component;
            }

            return null;
        }

        private void UpdateFacing(int targetSlotIndex)
        {
            IHeroModel hero = this.GetModel<IHeroModel>();
            this.SendCommand(new SetFacingCommand(targetSlotIndex > hero.CurSlotIndex.Value));
        }

        private bool IsTargetingCard()
        {
            return IsEnemyTargetCard() || IsSlotTargetCard();
        }

        private bool IsEnemyTargetCard()
        {
            return mCardUI.CardData.NeedsEnemyTarget;
        }

        private bool IsSlotTargetCard()
        {
            return mCardUI.CardData.NeedsSlotTarget;
        }

        private void CreateDragGhost(Vector2 position)
        {
            mDragGhost = Instantiate(gameObject, mTargetCanvas.transform);
            mDragGhost.transform.position = position;
            mDragGhost.transform.SetAsLastSibling();

            CanvasGroup cg = mDragGhost.GetComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            HandDragHandler handler = mDragGhost.GetComponent<HandDragHandler>();
            Destroy(handler);
        }

        private void DestroyDragGhost()
        {
            Destroy(mDragGhost);
            mDragGhost = null;
        }
    }
}