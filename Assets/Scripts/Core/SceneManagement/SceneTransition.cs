using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Core.SceneManagement
{
    public class SceneTransition : ISceneTransition
    {
        private readonly Image mTransitionImage;

        public SceneTransition(Image transitionImage)
        {
            mTransitionImage = transitionImage;
        }

        public async UniTask FadeOut(float duration = 0.3f)
        {
            if (mTransitionImage == null)
                return;

            mTransitionImage.gameObject.SetActive(true);
            mTransitionImage.raycastTarget = true;
            mTransitionImage.color = new Color(0f, 0f, 0f, 0f);

            UniTaskCompletionSource tcs = new();
            mTransitionImage.DOFade(1f, duration).SetEase(Ease.Linear)
                .OnComplete(() => tcs.TrySetResult());
            await tcs.Task;
        }

        public async UniTask FadeIn(float duration = 0.3f)
        {
            if (mTransitionImage == null)
                return;

            mTransitionImage.raycastTarget = true;

            UniTaskCompletionSource tcs = new();
            mTransitionImage.DOFade(0f, duration).SetEase(Ease.Linear)
                .OnComplete(() => tcs.TrySetResult());
            await tcs.Task;

            mTransitionImage.gameObject.SetActive(false);
            mTransitionImage.raycastTarget = false;
        }

        public void SetImmediate(bool black)
        {
            if (mTransitionImage == null)
                return;

            mTransitionImage.gameObject.SetActive(black);
            mTransitionImage.raycastTarget = black;
            mTransitionImage.color = new Color(0f, 0f, 0f, black ? 1f : 0f);
        }
    }
}