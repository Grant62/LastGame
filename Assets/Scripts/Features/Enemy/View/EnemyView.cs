using System.Collections.Generic;
using Core.Architecture;
using DG.Tweening;
using Features.Combat.Command;
using Features.Combat.Targeting;
using Features.Combat.View;
using Features.Enemy.Command;
using QFramework;
using UnityEngine;

namespace Features.Enemy.View
{
    public partial class EnemyView : ViewController, IController, IEnemyTarget, IDamageable
    {
        private int mHealth;
        private int mMaxHealth;
        private int mArmor;
        private Tween mHealthTween;

        [SerializeField] private ShieldView shieldView;

        public int MonsterId { get; private set; }
        public int Damage { get; private set; }
        public int SlotIndex { get; set; }

        public List<StatusModifier> Statuses { get; } = new();

        public Vector3 Position { get => transform.position; }

        public bool IsValidTarget { get => mHealth > 0; }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void Init(int monsterId, int maxHealth, int damage)
        {
            MonsterId = monsterId;
            mHealth = maxHealth;
            mMaxHealth = maxHealth;
            mArmor = 0;
            Damage = damage;
            RefreshHealthBar(false);
            shieldView.SetArmor(0);
        }

        public void TakeDamage(int amount)
        {
            this.SendCommand(new EnemyTakeDamageCommand(this, amount));
        }

        public void ApplyDamage(int amount)
        {
            if (amount <= 0)
                return;

            int remaining = amount;

            if (mArmor > 0)
            {
                int absorbed = Mathf.Min(mArmor, remaining);
                mArmor -= absorbed;
                remaining -= absorbed;
                shieldView.SetArmor(mArmor);
            }

            if (remaining > 0)
            {
                mHealth -= remaining;
                mHealth = Mathf.Max(0, mHealth);
                RefreshHealthBar(true);

                if (mHealth <= 0)
                {
                    gameObject.SetActive(false);
                    this.SendCommand(new SendEnemyDiedCommand(SlotIndex));
                }
            }
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