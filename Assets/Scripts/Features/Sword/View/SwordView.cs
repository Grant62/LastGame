using Core.Architecture;
using DG.Tweening;
using Features.Combat.View.Board;
using Features.Sword.Model;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Sword.View
{
    public class SwordView : MonoBehaviour, IController
    {
        [FoldoutGroup("移动")]
        [SerializeField] private float yOffset = 80f;

        public float YOffset { get => yOffset; }

        [FoldoutGroup("移动")]
        [SerializeField] private float skyY = 1000f;
        [FoldoutGroup("移动")]
        [SerializeField] private float moveDuration = 0.3f;
        [FoldoutGroup("移动")]
        [SerializeField] private float entryDuration = 0.6f;

        [FoldoutGroup("视觉")]
        [SerializeField] private Image swordImage;
        [FoldoutGroup("视觉")]
        [SerializeField] private float spinSpeed = 720f;
        [FoldoutGroup("视觉")]
        [SerializeField] private float spiritAlpha = 0.4f;

        private BoardView mBoard;
        private ISwordModel mSwordModel;
        private Tween mMoveTween;
        private Tween mSpinTween;
        private bool mFirstMove = true;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            transform.position = new Vector3(0, skyY, 0);
        }

        public void Init(BoardView board, bool listenToModel = true)
        {
            mBoard = board;
            mSwordModel = this.GetModel<ISwordModel>();

            if (!listenToModel)
                return;

            mSwordModel.CurSlotIndex.Register(OnSwordSlotChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            mSwordModel.IsSpinning.Register(OnSpinStateChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            mSwordModel.IsSpiritAttached.Register(OnSpiritAttachedChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnSwordSlotChanged(int slotIndex)
        {
            if (slotIndex < 0)
                return;

            RectTransform slot = mBoard.GetSlotTransform(slotIndex);
            Vector3 targetPos = slot.position + new Vector3(0, yOffset, 0);

            mMoveTween?.Kill();
            float duration = mFirstMove ? entryDuration : moveDuration;
            mMoveTween = transform.DOMove(targetPos, duration).SetEase(Ease.OutCubic);
            mFirstMove = false;
        }

        private void OnSpinStateChanged(bool spinning)
        {
            if (spinning)
            {
                mSpinTween?.Kill();
                float duration = 360f / spinSpeed;
                mSpinTween = swordImage.rectTransform
                    .DOLocalRotate(new Vector3(0, 0, -360f), duration, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Incremental)
                    .SetEase(Ease.Linear);
            }
            else
            {
                mSpinTween?.Kill();
                mSpinTween = null;
                swordImage.rectTransform.localEulerAngles = Vector3.zero;
            }
        }

        private void OnSpiritAttachedChanged(bool attached)
        {
            Color c = swordImage.color;
            c.a = attached ? spiritAlpha : 1f;
            swordImage.color = c;
        }

        public void SetColor(Color color)
        {
            swordImage.color = color;
        }
    }
}