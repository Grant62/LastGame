using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Presentation.Effects
{
    public class DamageTextUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private CanvasGroup canvasGroup;

        [BoxGroup("动画")]
        [SerializeField] private float duration = 0.8f;
        [BoxGroup("动画")]
        [SerializeField] private float moveY = 200f;
        [BoxGroup("动画")]
        [SerializeField] private float moveXRandom = 30f;
        [BoxGroup("动画")]
        [SerializeField] private float startOffsetY = 50f;

        private Sequence mSeq;

        public void Play(string value, Color color, Vector2 screenPos, Action onDone)
        {
            Vector3 pos = screenPos;
            pos.x += Random.Range(-30f, 30f);
            pos.y += startOffsetY;

            transform.position = pos;
            text.text = value;
            text.color = color;
            canvasGroup.alpha = 1f;

            float startScale = Random.Range(1f, 1.3f);
            transform.localScale = Vector3.one * startScale;
            transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-5f, 5f));
            float endX = pos.x + Random.Range(-moveXRandom, moveXRandom);

            mSeq?.Kill();
            mSeq = DOTween.Sequence();
            mSeq.Join(transform.DOScale(startScale * 1.15f, 0.12f).SetEase(Ease.OutBack));
            mSeq.Join(transform.DOMoveX(endX, duration).SetEase(Ease.InOutQuad));
            mSeq.Join(transform.DOMoveY(pos.y + moveY, duration).SetEase(Ease.InOutQuad));
            mSeq.Join(canvasGroup.DOFade(0f, duration * 0.55f).SetDelay(duration * 0.35f));
            mSeq.OnComplete(() => onDone?.Invoke());
        }
    }
}