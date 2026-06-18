using System.Collections.Generic;
using Core.Architecture;
using DG.Tweening;
using Features.Card.Command;
using Features.Card.Data;
using Features.Card.Interfaces;
using Features.Card.UI;
using Features.Combat.System;
using Features.Combat.Targeting;
using Features.Combat.Targeting.Command;
using Features.Hero.Command;
using Features.Hero.Model;
using Features.Resource.System;
using Features.Sword.Command;
using Features.Sword.Model;
using Features.Sword.System;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Card.View
{
    public class HandDragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IController
    {
        private ISlotTargetSystem mSlotSystem;
        private CardView mCardView;
        private RectTransform mLayoutRoot;
        private RectTransform mRectTransform;
        private CanvasGroup mCanvasGroup;
        private bool mIsDragging;
        private Vector2 mOriginalPos;
        private Vector3 mOriginalRotation;
        private int mOriginalSiblingIndex;
        private Vector2 mDragOffset;

        [SerializeField] private float dragReleaseYThreshold = 300f;
        [SerializeField] private float snapBackDuration = 0.15f;

        private readonly PointerEventData mCachedPed = new(EventSystem.current);
        private readonly List<RaycastResult> mCachedResults = new();

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            mCardView = GetComponent<CardView>();
            mLayoutRoot = GetComponentInParent<HandPanel>()?.LayoutRoot;
            mRectTransform = GetComponent<RectTransform>();
            mCanvasGroup = GetComponentInChildren<CanvasGroup>();
            mSlotSystem = this.GetSystem<ISlotTargetSystem>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!this.GetSystem<IInteractionSystem>().CanInteract())
                return;

            if (!this.GetSystem<IResourceSystem>().CanSpend(mCardView.CardData.Cost))
                return;

            mIsDragging = true;
            this.GetSystem<IInteractionSystem>().BeginDrag();

            mLayoutRoot = GetComponentInParent<HandPanel>().LayoutRoot;

            mRectTransform.DOKill();
            mOriginalPos = mRectTransform.anchoredPosition;
            mOriginalRotation = mRectTransform.localEulerAngles;
            mOriginalSiblingIndex = mRectTransform.GetSiblingIndex();
            mRectTransform.SetAsLastSibling();

            if (IsTargetingCard())
            {
                this.SendCommand(new StartTargetingCommand(transform.position));
            }
            else
            {
                GetArchitecture().GetUtility<ICardHoverDisplay>().Hide();
                mCanvasGroup.alpha = 1f;
                mRectTransform.localEulerAngles = Vector3.zero;

                Vector2 cardScreenPos = RectTransformUtility.WorldToScreenPoint(null, mRectTransform.position);
                mDragOffset = cardScreenPos - eventData.position;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!mIsDragging)
                return;

            mIsDragging = false;
            this.GetSystem<IInteractionSystem>().EndDrag();
            this.SendCommand<EndTargetingCommand>();

            if (IsTargetingCard())
            {
                GetArchitecture().GetUtility<ICardHoverDisplay>().Hide();
                mCanvasGroup.alpha = 1f;
            }

            bool played;

            if (IsEnemyTargetCard())
                played = PlayWithEnemyTarget(eventData.position);
            else if (IsSlotTargetCard())
                played = PlayWithSlotTarget(eventData.position);
            else
                played = PlayNormal(eventData.position);

            if (!played)
            {
                if (IsTargetingCard())
                {
                    mRectTransform.SetSiblingIndex(mOriginalSiblingIndex);
                    mRectTransform.localEulerAngles = mOriginalRotation;
                }
                else
                {
                    SnapBack();
                }
            }
        }

        private void Update()
        {
            if (!mIsDragging || IsTargetingCard())
                return;

            Vector2 target = (Vector2)Input.mousePosition + mDragOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mLayoutRoot, target, null, out Vector2 localPos);
            mRectTransform.anchoredPosition = localPos;
        }

        private void SnapBack()
        {
            mRectTransform.DOAnchorPos(mOriginalPos, snapBackDuration).SetEase(Ease.OutCubic);
            mRectTransform.DOLocalRotate(mOriginalRotation, snapBackDuration).SetEase(Ease.OutCubic);
            mRectTransform.SetSiblingIndex(mOriginalSiblingIndex);
        }

        private bool PlayWithEnemyTarget(Vector2 screenPos)
        {
            IEnemyTarget enemyTarget = RaycastFor<IEnemyTarget>(screenPos);
            if (enemyTarget == null || !enemyTarget.IsValidTarget)
                return false;

            SlotAction action = mCardView.CardData.SlotAction;

            this.SendCommand(new PlayCardCommand(mCardView.CardData, enemyTarget));

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
            if (!mSlotSystem.Validate(mCardView.CardData, slotIndex))
                return false;

            SlotAction action = mCardView.CardData.SlotAction;

            this.SendCommand(new PlayCardCommand(mCardView.CardData, null, slotIndex));
            UpdateFacing(slotIndex);

            if (action == SlotAction.MoveSword)
                this.SendCommand(new MoveSwordCommand(slotIndex));
            else if (action == SlotAction.MovePlayer)
                this.SendCommand(new MovePlayerCommand(slotIndex));

            return true;
        }

        private bool PlayNormal(Vector2 screenPos)
        {
            if (screenPos.y < dragReleaseYThreshold)
                return false;

            this.SendCommand(new PlayCardCommand(mCardView.CardData));
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
            return mCardView.CardData.NeedsEnemyTarget;
        }

        private bool IsSlotTargetCard()
        {
            if (!mCardView.CardData.NeedsSlotTarget)
                return false;

            if (mCardView.CardData.SlotAction == SlotAction.SpawnSpiritAtSlot)
                return this.GetModel<ISwordModel>().SpiritSwordSlots.Count > 0;

            return true;
        }
    }
}