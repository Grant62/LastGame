using Core.Architecture;
using DG.Tweening;
using Features.Combat;
using Features.Enemy.View;
using Features.Hero.Model;
using Features.Sword.Model;
using QFramework;
using UnityEngine;

namespace Features.Sword.View
{
    public class SwordView : MonoBehaviour, IController
    {
        [SerializeField] private Vector2 playerOffset = new(-0.5f, 0.5f);
        [SerializeField] private Vector2 enemyOffset = new(0.5f, 0.5f);
        [SerializeField] private float moveDuration = 0.3f;

        private BoardView mBoard;
        private ISwordModel mSwordModel;
        private IHeroModel mHeroModel;
        private Tween mMoveTween;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void Init(BoardView board)
        {
            mBoard = board;
            mSwordModel = this.GetModel<ISwordModel>();
            mHeroModel = this.GetModel<IHeroModel>();

            mSwordModel.CurrentSlotIndex.Register(OnSwordSlotChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnSwordSlotChanged(int slotIndex)
        {
            if (slotIndex < 0)
                return;

            Transform slot = mBoard.GetSlotTransform(slotIndex);
            if (slot == null)
                return;

            Vector3 targetPos = CalculateTargetPosition(slotIndex, slot);
            AnimateTo(targetPos);
        }

        private Vector3 CalculateTargetPosition(int slotIndex, Transform slot)
        {
            if (mBoard.TryGetEnemyAtSlot(slotIndex, out EnemyView enemy))
            {
                float side = slotIndex > mHeroModel.CurrentSlotIndex.Value ? 1f : -1f;
                return enemy.transform.position + new Vector3(side * Mathf.Abs(enemyOffset.x), enemyOffset.y, 0);
            }

            if (slotIndex == mHeroModel.CurrentSlotIndex.Value)
            {
                float behind = mHeroModel.IsFacingRight.Value ? -1f : 1f;
                return slot.position + new Vector3(behind * Mathf.Abs(playerOffset.x), playerOffset.y, 0);
            }

            return slot.position;
        }

        private void AnimateTo(Vector3 target)
        {
            mMoveTween?.Kill();
            mMoveTween = transform.DOMove(target, moveDuration).SetEase(Ease.OutCubic);
        }
    }
}