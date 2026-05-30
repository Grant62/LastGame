using Core.Architecture;
using Cysharp.Threading.Tasks;
using Features.Card.Command;
using Features.Card.Event;
using Features.Card.Model;
using Features.Combat;
using Features.Combat.Interaction;
using Features.Combat.Targeting;
using Features.Combat.Targeting.Command;
using Features.Hero.Command;
using Features.Hero.Model;
using Features.Hero.View;
using Features.Sword.Command;
using Features.Sword.System;
using QFramework;
using Services.SlotDetector;
using UnityEngine;

namespace Features.Card.View
{
    public class CardDragHandler : MonoBehaviour, IController, ICanSendEvent
    {
        private static readonly int OutlineWidthId = Shader.PropertyToID("_OutlineWidth");
        private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineSizeId = Shader.PropertyToID("_OutlineSize");

        [SerializeField] private LayerMask dropLayer;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private Material outlineMaterial;
        [SerializeField] private float hoverY = -4f;

        private CardView mCardView;
        private MaterialPropertyBlock mMpb;
        private Vector3 mDragStartPos;
        private Quaternion mDragStartRot;
        private bool mIsDragging;
        private bool mIsTweening;
        private HandView mHandView;
        private ISlotDetector mSlotDetector;
        private ISlotTargetSystem mSlotSystem;
        private BoardView mCachedBoard;
        private HeroView mCachedHero;
        private Camera mMainCamera;

        private CardView CardView { get => mCardView ??= GetComponent<CardView>(); }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private bool mCardNeedsEnemyTarget { get => CardView.CardData.ManualTargetEffect is { Count: > 0 }; }

        private bool mCardNeedsSlotTarget
        {
            get
            {
                int id = CardView.CardData.CardId;
                return id is 12002 or 12003;
            }
        }

        private bool mCardNeedsTargeting { get => mCardNeedsEnemyTarget || mCardNeedsSlotTarget; }

        public void SetUsable(bool usable)
        {
            mMpb ??= new MaterialPropertyBlock();
            CardView.CardImage.GetPropertyBlock(mMpb);

            if (CardView.CardImage.sharedMaterial != outlineMaterial)
                CardView.CardImage.sharedMaterial = outlineMaterial;
            if (usable)
            {
                mMpb.SetFloat(OutlineWidthId, outlineMaterial.GetFloat(OutlineWidthId));
                mMpb.SetColor(OutlineColorId, outlineMaterial.GetColor(OutlineColorId));
                mMpb.SetFloat(OutlineSizeId, outlineMaterial.GetFloat(OutlineSizeId));
            }
            else
                mMpb.SetFloat(OutlineWidthId, 0);

            CardView.CardImage.SetPropertyBlock(mMpb);
        }

        public void SetTweening(bool isTweening)
        {
            mIsTweening = isTweening;
        }

        public void ResetWrapper()
        {
            CardView.Wrapper.gameObject.SetActive(true);
        }

        private void Start()
        {
            mMainCamera = Camera.main;
            mHandView = GetComponentInParent<HandView>();
            mSlotDetector = this.GetUtility<ISlotDetector>();
            mSlotSystem = this.GetSystem<ISlotTargetSystem>();
            mCachedBoard = FindFirstObjectByType<BoardView>();
            mCachedHero = FindFirstObjectByType<HeroView>();
        }

        private void OnMouseEnter()
        {
            if (mIsTweening || mIsDragging || this.GetSystem<IInteractionSystem>().IsDragging)
                return;

            if (!this.GetSystem<IInteractionSystem>().CanHover())
                return;

            CardView.Wrapper.gameObject.SetActive(false);

            Vector3 pos = new(CardView.transform.position.x, hoverY, 0);
            this.SendEvent(new CardHoverEvent(CardView.CardData, pos));
        }

        private void OnMouseExit()
        {
            if (mIsDragging || this.GetSystem<IInteractionSystem>().IsDragging)
                return;

            this.SendEvent(new CardHoverEndEvent());
            CardView.Wrapper.gameObject.SetActive(true);
        }

        private void OnMouseDown()
        {
            if (mIsTweening)
                return;

            if (!this.GetSystem<IInteractionSystem>().CanInteract())
                return;

            mIsDragging = true;
            this.SendEvent<DragStartEvent>();
            CardView.SortingGroup.sortingOrder = 999;

            mDragStartPos = CardView.transform.position;
            mDragStartRot = CardView.transform.rotation;

            if (mCardNeedsTargeting)
            {
                this.SendCommand(new StartTargetingCommand(mDragStartPos));
            }
            else
            {
                CardView.Wrapper.gameObject.SetActive(true);
                this.SendEvent(new CardHoverEndEvent());
                CardView.transform.rotation = Quaternion.Euler(0, 0, 0);
                CardView.transform.position = GetMouseWorldPosition();
            }
        }

        private void OnMouseDrag()
        {
            if (!mIsDragging)
                return;

            if (!mCardNeedsTargeting)
                CardView.transform.position = GetMouseWorldPosition();
        }

        private void OnMouseUp()
        {
            if (!mIsDragging)
                return;

            mIsDragging = false;
            this.SendEvent<DragEndEvent>();
            this.SendCommand<EndTargetingCommand>();
            this.SendEvent(new CardHoverEndEvent());

            Vector3 mousePos = GetMouseWorldPosition();
            bool played = false;

            if (mCardNeedsEnemyTarget)
                played = PlayWithEnemyTarget(mousePos);
            else if (mCardNeedsSlotTarget)
                played = PlayWithSlotTarget(mousePos);
            else
                played = PlayNormal(mousePos);

            if (!played || this.GetModel<ICardModel>().HandPile.Contains(CardView.CardData))
            {
                CardView.transform.position = mDragStartPos;
                CardView.transform.rotation = mDragStartRot;
                CardView.Wrapper.gameObject.SetActive(true);

                if (mHandView != null)
                    mHandView.SetCardLayoutAsync(0.15f).Forget();
            }
        }

        private bool PlayWithEnemyTarget(Vector3 mousePos)
        {
            Vector3 rayOrigin = new(mousePos.x, mousePos.y, -1f);
            if (Physics.Raycast(rayOrigin, Vector3.forward, out RaycastHit hit, 5f, enemyLayer)
                && hit.collider.TryGetComponent(out IDamageable target))
            {
                this.SendCommand(new PlayCardCommand(CardView.CardData, target));

                if (mCachedBoard != null && target is MonoBehaviour mb && mb.transform.parent != null)
                {
                    int slotIndex = mCachedBoard.GetSlotIndex(mb.transform.parent);
                    if (slotIndex >= 0)
                        UpdateFacing(slotIndex);
                }

                return true;
            }

            return false;
        }

        private bool PlayWithSlotTarget(Vector3 mousePos)
        {
            int slotIndex = mSlotDetector.GetSlotIndexAtPosition(mousePos);
            if (slotIndex < 0)
                return false;

            if (!mSlotSystem.Validate(CardView.CardData.CardId, CardView.CardData.Desc, slotIndex))
                return false;
            this.SendCommand(new PlayCardCommand(CardView.CardData));

            UpdateFacing(slotIndex);

            int cardId = CardView.CardData.CardId;
            if (cardId == 12002)
                this.SendCommand(new MoveSwordCommand(slotIndex));
            else if (cardId == 12003 && mCachedBoard != null && mCachedHero != null)
            {
                this.SendCommand(new MovePlayerCommand(slotIndex));
            }

            return true;
        }

        private void UpdateFacing(int targetSlotIndex)
        {
            IHeroModel hero = this.GetModel<IHeroModel>();
            this.SendCommand(new SetFacingCommand(targetSlotIndex > hero.CurrentSlotIndex.Value));
        }

        private bool PlayNormal(Vector3 mousePos)
        {
            if (Physics2D.OverlapPoint(mousePos, dropLayer))
            {
                this.SendCommand(new PlayCardCommand(CardView.CardData));
                return true;
            }

            return false;
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -mMainCamera.transform.position.z;
            return mMainCamera.ScreenToWorldPoint(mousePos);
        }
    }
}