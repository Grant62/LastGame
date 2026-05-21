using Core.Architecture;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Features.Card.Data;
using Features.Card.Interfaces;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Features.Card.View
{
    public class CardView : MonoBehaviour, ICardDisplayBase, IController
    {
        private static readonly int OutlineWidthId = Shader.PropertyToID("_OutlineWidth");
        private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineSizeId = Shader.PropertyToID("_OutlineSize");

        [SerializeField] private TMP_Text mTitle;
        [SerializeField] private TMP_Text mDesc;
        [SerializeField] private TMP_Text mCost;
        [SerializeField] private TMP_Text mType;
        [SerializeField] private GameObject mWrapper;
        [SerializeField] private LayerMask mDropLayer;
        [SerializeField] private LayerMask mEnemyLayer;
        [SerializeField] private Material mOutlineMaterial;
        [SerializeField] private SpriteRenderer mCardImageSr;

        public CardData CardData { get; private set; }

        private MaterialPropertyBlock mMpb;
        private SortingGroup mSortingGroup;

        public SortingGroup SortingGroup => mSortingGroup ??= GetComponent<SortingGroup>();

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void Setup(CardData data)
        {
            SetCardData(data);
            SetTitle(data.Name);
            SetDescription(data.Desc);
            SetCost(data.Cost.ToString());
            SetType(data.Rarity);
            LoadImageAsync(data).Forget();
        }

        private async UniTaskVoid LoadImageAsync(CardData data)
        {
        }

        public void SetCardData(CardData data)
        {
            CardData = data;
        }

        public void SetTitle(string titleText)
        {
            mTitle.text = titleText;
        }

        public void SetDescription(string description)
        {
            mDesc.text = description;
        }

        public void SetCost(string costText)
        {
            mCost.text = costText;
        }

        public void SetType(string typeText)
        {
            mType.text = typeText;
        }

        public void SetRarityColor(Color color)
        {
            mTitle.color = color;
        }

        public void SetImage(Sprite sprite)
        {
            mCardImageSr.sprite = sprite;
        }

        public void UpdatePositionRotation(Vector3 pos, Quaternion rot, float duration)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMove(pos, duration));
            seq.Join(transform.DORotate(rot.eulerAngles, duration));
        }

        public void SetUsable(bool usable)
        {
            mMpb ??= new MaterialPropertyBlock();
            mCardImageSr.GetPropertyBlock(mMpb);

            if (usable)
            {
                if (mCardImageSr.sharedMaterial != mOutlineMaterial)
                    mCardImageSr.sharedMaterial = mOutlineMaterial;

                mMpb.SetFloat(OutlineWidthId, mOutlineMaterial.GetFloat(OutlineWidthId));
                mMpb.SetColor(OutlineColorId, mOutlineMaterial.GetColor(OutlineColorId));
                mMpb.SetFloat(OutlineSizeId, mOutlineMaterial.GetFloat(OutlineSizeId));
            }
            else
            {
                if (mCardImageSr.sharedMaterial != mOutlineMaterial)
                    mCardImageSr.sharedMaterial = mOutlineMaterial;

                mMpb.SetFloat(OutlineWidthId, 0);
            }

            mCardImageSr.SetPropertyBlock(mMpb);
        }

        public void UpdateUsable()
        {
        }

        public void ResetWrapper()
        {
        }
    }
}
