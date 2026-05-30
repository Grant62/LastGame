using DG.Tweening;
using Features.Combat;
using Features.Combat.Targeting;
using QFramework;
using UnityEngine;

namespace Features.Enemy.View
{
    public partial class EnemyView : ViewController, IDamageable
    {
        private int mHealth;
        private int mMaxHealth;
        private Tween mHealthTween;

        public int MonsterId { get; private set; }
        public int Damage { get; private set; }

        public Vector3 Position { get => transform.position; }

        public bool IsValidTarget { get => mHealth > 0; }

        public void TakeDamage(int amount)
        {
            if (amount <= 0)
                return;

            mHealth -= amount;
            mHealth = Mathf.Max(0, mHealth);
            RefreshHealthBar(true);

            if (mHealth <= 0)
            {
                BoardView board = GetComponentInParent<BoardView>();
                board.RemoveEnemy(this);
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

        public void Init(int monsterId, int maxHealth, int damage)
        {
            MonsterId = monsterId;
            mHealth = maxHealth;
            mMaxHealth = maxHealth;
            Damage = damage;
            RefreshHealthBar();
        }

        private void RefreshHealthBar(bool animate = false)
        {
            mHealthTween?.Kill();
            float ratio = mMaxHealth > 0 ? (float)mHealth / mMaxHealth : 0f;
            if (animate)
                mHealthTween = Fill.transform.DOScaleX(ratio, 0.3f);
            else
                Fill.transform.localScale = new Vector3(ratio, 1, 1);
        }
    }
}