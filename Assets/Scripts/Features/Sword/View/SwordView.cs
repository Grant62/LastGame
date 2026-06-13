using Core.Architecture;
using DG.Tweening;
using Features.Combat.View.Board;
using Features.Sword.Model;
using QFramework;
using UnityEngine;

namespace Features.Sword.View
{
    public class SwordView : MonoBehaviour, IController
    {
        [SerializeField] private float yOffset = 80f;

        public float YOffset { get => yOffset; }

        [SerializeField] private float skyY = 1000f;
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private float entryDuration = 0.6f;

        private BoardView mBoard;
        private ISwordModel mSwordModel;
        private Tween mMoveTween;
        private bool mFirstMove = true;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            transform.position = new Vector3(0, skyY, 0);
        }

        public void Init(BoardView board)
        {
            mBoard = board;
            mSwordModel = this.GetModel<ISwordModel>();

            mSwordModel.CurSlotIndex.Register(OnSwordSlotChanged)
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
    }
}