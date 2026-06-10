using System.Collections.Generic;
using Core.Architecture;
using DG.Tweening;
using Features.Combat.Event;
using Features.Combat.Targeting;
using QFramework;
using UnityEngine;

namespace Features.Combat.UI.Board
{
    public partial class EnemyUI : ViewController, IController, IEnemyTarget, IDamageable
    {
        private int mHealth;
        private int mMaxHealth;
        private Tween mHealthTween;

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
            Damage = damage;
            RefreshHealthBar(false);
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0)
                return;

            mHealth -= amount;
            mHealth = Mathf.Max(0, mHealth);
            RefreshHealthBar(true);

            if (mHealth <= 0)
            {
                gameObject.SetActive(false);
                GameMain.Interface.SendEvent(new EnemyDiedEvent(SlotIndex));
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