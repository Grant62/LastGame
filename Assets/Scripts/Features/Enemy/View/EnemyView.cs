using System.Collections.Generic;
using Core.Architecture;
using DG.Tweening;
using Features.Combat.Command;
using Features.Combat.Define;
using Features.Combat.Interfaces;
using Features.Combat.Utility;
using Features.Combat.View;
using Features.Enemy.Command;
using Features.Enemy.Define;
using Features.Enemy.Event;
using Features.Hero.Model;
using QFramework;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Enemy.View
{
    public partial class EnemyView : ViewController, IController, IEnemyTarget, IDamageable
    {
        private int mHealth;
        private int mMaxHealth;
        private int mArmor;
        private Tween mHealthTween;
        private SkeletonGraphic mSkeleton;
        private Transform mSpineTrans;

        [SerializeField] private ShieldView shieldView;
        [SerializeField] private GameObject intentRoot;
        [SerializeField] private Image intentIcon;
        [SerializeField] private Sprite attackIconSprite;
        [SerializeField] private Sprite moveIconSprite;
        [SerializeField] private Sprite moveAttackIconSprite;

        public int SlotIndex { get; set; }

        public List<StatusModifier> Statuses { get; } = new();

        public Vector3 Position { get => transform.position; }

        public bool IsValidTarget { get => mHealth > 0; }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void Init(int maxHealth)
        {
            mHealth = maxHealth;
            mMaxHealth = maxHealth;
            mArmor = 0;

            if (HealthBarFill == null || HealthText == null || shieldView == null) return;

            RefreshHealthBar(false);
            shieldView.SetArmor(0);

            this.RegisterEvent<EnemyIntentEvent>(e =>
            {
                if (e.SlotIndex == SlotIndex)
                {
                    bool flip = e.SlotIndex > GameMain.Interface.GetModel<IHeroModel>().CurSlotIndex.Value;
                    ShowIntent(e.Intent, flip);
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            if (mSkeleton == null)
            {
                mSkeleton = GetComponentInChildren<SkeletonGraphic>();
                if (mSkeleton != null)
                    mSpineTrans = mSkeleton.transform;
            }

            if (mSkeleton != null && mSkeleton.AnimationState != null)
            {
                mSkeleton.AnimationState.ClearTracks();
                mSkeleton.AnimationState.SetAnimation(0, "ready", true);
            }
        }

        public void SetFacing(bool faceRight)
        {
            if (mSpineTrans == null)
            {
                mSpineTrans = transform.Find("Spine");
                if (mSpineTrans == null)
                    return;
            }

            Vector3 scale = mSpineTrans.localScale;
            scale.x = Mathf.Abs(scale.x) * (faceRight ? 1f : -1f);
            mSpineTrans.localScale = scale;
        }

        public void ShowIntent(EnemyIntentType intent, bool flipArrow = false)
        {
            switch (intent)
            {
                case EnemyIntentType.Attack:
                    intentIcon.sprite = attackIconSprite;
                    break;
                case EnemyIntentType.Move:
                    intentIcon.sprite = moveIconSprite;
                    break;
                case EnemyIntentType.MoveAttack:
                    intentIcon.sprite = moveAttackIconSprite;
                    break;
                default:
                    intentIcon.sprite = null;
                    break;
            }

            bool visible = intentIcon.sprite != null;
            intentRoot.SetActive(visible);

            if (visible)
            {
                Vector3 scale = intentIcon.transform.localScale;
                scale.x = flipArrow ? -1f : 1f;
                intentIcon.transform.localScale = scale;
            }
        }

        public void PlayAttack()
        {
            if (mSkeleton != null)
            {
                TrackEntry track = mSkeleton.AnimationState.SetAnimation(0, "hit1", false);
                track.Complete += _ => mSkeleton.AnimationState.SetAnimation(0, "ready", true);
            }
        }

        public void TakeDamage(int amount)
        {
            this.SendCommand(new EnemyTakeDamageCommand(this, amount));
        }

        public void ApplyDamage(int amount)
        {
            if (amount <= 0)
                return;

            if (mHealth <= 0)
                return;

            int remaining = amount;

            if (mArmor > 0)
            {
                int absorbed = Mathf.Min(mArmor, remaining);
                mArmor -= absorbed;
                remaining -= absorbed;
                shieldView.SetArmor(mArmor);
                if (absorbed > 0)
                    this.GetUtility<IDamageTextSpawner>().Spawn($"-{absorbed}", transform.position, Color.white);
            }

            if (remaining > 0)
            {
                mHealth -= remaining;
                mHealth = Mathf.Max(0, mHealth);
                RefreshHealthBar(true);
                this.GetUtility<IDamageTextSpawner>().Spawn(remaining, transform.position, Color.red);

                if (mSkeleton != null)
                {
                    TrackEntry track = mSkeleton.AnimationState.SetAnimation(0, "hit2", false);
                    track.Complete += _ => mSkeleton.AnimationState.SetAnimation(0, "ready", true);
                }

                if (mHealth <= 0)
                {
                    KillEnemy();
                }
            }
        }

        private void KillEnemy()
        {
            if (mSkeleton != null)
            {
                TrackEntry dieTrack = mSkeleton.AnimationState.SetAnimation(0, "dead", false);
                dieTrack.Complete += _ => { DoCleanup(); };
            }
            else
            {
                DoCleanup();
            }
        }

        private void DoCleanup()
        {
            gameObject.SetActive(false);
            this.SendCommand(new SendEnemyDiedCommand(SlotIndex));
        }

        public void TakeHeal(int amount)
        {
            if (amount <= 0)
                return;

            mHealth += amount;
            mHealth = Mathf.Min(mMaxHealth, mHealth);
            RefreshHealthBar(true);
        }

        public void GainArmor(int amount)
        {
            this.SendCommand(new EnemyGainArmorCommand(this, amount));
        }

        public void ApplyArmor(int amount)
        {
            if (amount <= 0)
                return;

            mArmor += amount;
            shieldView.SetArmor(mArmor);
        }

        private void RefreshHealthBar(bool animate)
        {
            mHealthTween?.Kill();
            float ratio = mMaxHealth > 0 ? (float)mHealth / mMaxHealth : 0f;

            if (animate)
                mHealthTween = HealthBarFill.DOFillAmount(ratio, 0.3f);
            else
                HealthBarFill.fillAmount = ratio;

            HealthText.text = mHealth <= 0 ? "死亡" : $"{mHealth}/{mMaxHealth}";
        }
    }
}