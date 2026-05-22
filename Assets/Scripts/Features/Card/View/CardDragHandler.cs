using Core.Architecture;
using Features.Card.Command;
using Features.Card.Event;
using QFramework;
using UnityEngine;

namespace Features.Card.View
{
    public class CardDragHandler : MonoBehaviour, IController, ICanSendEvent
    {
        private static readonly int OutlineWidthId = Shader.PropertyToID("_OutlineWidth");
        private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineSizeId = Shader.PropertyToID("_OutlineSize");

        [SerializeField] private LayerMask mDropLayer;
        [SerializeField] private LayerMask mEnemyLayer;
        [SerializeField] private Material mOutlineMaterial;

        private CardView mCardView;
        private MaterialPropertyBlock mMpb;
        private Vector3 mDragStartPos;
        private Quaternion mDragStartRot;
        private bool mIsDragging;
        private bool mIsTweening;

        private CardView CardView { get => mCardView ??= GetComponent<CardView>(); }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void SetUsable(bool usable)
        {
            mMpb ??= new MaterialPropertyBlock();
            CardView.CardImage.GetPropertyBlock(mMpb);

            if (usable)
            {
                if (CardView.CardImage.sharedMaterial != mOutlineMaterial)
                    CardView.CardImage.sharedMaterial = mOutlineMaterial;

                mMpb.SetFloat(OutlineWidthId, mOutlineMaterial.GetFloat(OutlineWidthId));
                mMpb.SetColor(OutlineColorId, mOutlineMaterial.GetColor(OutlineColorId));
                mMpb.SetFloat(OutlineSizeId, mOutlineMaterial.GetFloat(OutlineSizeId));
            }
            else
            {
                if (CardView.CardImage.sharedMaterial != mOutlineMaterial)
                    CardView.CardImage.sharedMaterial = mOutlineMaterial;

                mMpb.SetFloat(OutlineWidthId, 0);
            }

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

        private void OnMouseEnter()
        {
            if (mIsTweening)
                return;

            CardView.Wrapper.gameObject.SetActive(false);

            Vector3 pos = new(CardView.transform.position.x, -2.8f, 0);
            this.SendEvent(new CardHoverEvent(CardView.CardData, pos));
        }

        private void OnMouseExit()
        {
            this.SendEvent(new CardHoverEndEvent());
            CardView.Wrapper.gameObject.SetActive(true);
        }

        private void OnMouseDown()
        {
            if (mIsTweening)
                return;

            mIsDragging = true;
            CardView.Wrapper.gameObject.SetActive(true);
            this.SendEvent(new CardHoverEndEvent());

            mDragStartPos = CardView.transform.position;
            mDragStartRot = CardView.transform.rotation;

            CardView.transform.rotation = Quaternion.Euler(0, 0, 0);
            CardView.transform.position = GetMouseWorldPosition();
        }

        private void OnMouseDrag()
        {
            if (!mIsDragging)
                return;

            CardView.transform.position = GetMouseWorldPosition();
        }

        private void OnMouseUp()
        {
            if (!mIsDragging)
                return;

            mIsDragging = false;

            Vector3 mousePos = GetMouseWorldPosition();

            if (Physics.Raycast(mousePos, Vector3.forward, out RaycastHit hit, 10f, mDropLayer))
            {
                this.SendCommand(new PlayCardCommand(CardView.CardData));
            }
            else
            {
                CardView.transform.position = mDragStartPos;
                CardView.transform.rotation = mDragStartRot;
            }
        }

        private static Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }
    }
}