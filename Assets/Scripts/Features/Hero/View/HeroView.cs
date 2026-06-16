using Core.Architecture;
using DG.Tweening;
using Features.Combat.Targeting;
using Features.Combat.Utility;
using Features.Combat.View;
using Features.Combat.View.Board;
using Features.Hero.Command;
using Features.Hero.Model;
using QFramework;
using Spine.Unity;
using UnityEngine;

namespace Features.Hero.View
{
    public partial class HeroView : ViewController, IController, IDamageable
    {
        private IHeroModel mHeroModel;
        private Tween mHealthTween;
        private Transform mSpineTrans;
        private SkeletonGraphic mSkeleton;
        [SerializeField] private ShieldView shieldView;

        public Vector3 Position { get => transform.position; }

        public bool IsValidTarget { get => mHeroModel.Health.Value > 0; }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void TakeDamage(int amount)
        {
            this.SendCommand(new HeroTakeDamageCommand(amount));
        }

        public void TakeHeal(int amount)
        {
            this.SendCommand(new HeroTakeHealCommand(amount));
        }

        public void GainArmor(int amount)
        {
            this.SendCommand(new HeroGainArmorCommand(amount));
        }

        private void Start()
        {
            mHeroModel = this.GetModel<IHeroModel>();
            mSkeleton = GetComponentInChildren<SkeletonGraphic>();
            mSpineTrans = mSkeleton.transform;

            mSkeleton.AnimationState.SetAnimation(0, "appear", false)
                .Complete += _ => mSkeleton.AnimationState.SetAnimation(0, "ready", true);

            mHeroModel.Health.RegisterWithInitValue(_ => RefreshHealthBar(true))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            mHeroModel.MaxHealth.Register(_ => RefreshHealthBar(true))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            mHeroModel.IsFacingRight.RegisterWithInitValue(OnFacingChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            mHeroModel.CurSlotIndex.Register(OnSlotChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            mHeroModel.Armor.RegisterWithInitValue(shieldView.SetArmor)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnSlotChanged(int slotIndex)
        {
            BoardView board = GetArchitecture().GetUtility<IBoardAccess>().Board;
            RectTransform target = board.GetSlotTransform(slotIndex);
            transform.SetParent(target);
            transform.DOLocalMove(Vector3.zero, 0.15f).SetEase(Ease.OutCubic);
        }

        private void OnFacingChanged(bool facingRight)
        {
            Vector3 scale = mSpineTrans.localScale;
            scale.x = Mathf.Abs(scale.x) * (facingRight ? 1 : -1);
            mSpineTrans.localScale = scale;
        }

        private void RefreshHealthBar(bool animate = false)
        {
            mHealthTween?.Kill();
            float ratio = mHeroModel.MaxHealth.Value > 0
                ? (float)mHeroModel.Health.Value / mHeroModel.MaxHealth.Value
                : 0f;

            if (animate)
                mHealthTween = BarFill.DOFillAmount(ratio, 0.3f);
            else
                BarFill.fillAmount = ratio;

            Label.text = mHeroModel.Health.Value <= 0
                ? "死亡"
                : $"{mHeroModel.Health.Value}/{mHeroModel.MaxHealth.Value}";
        }
    }
}