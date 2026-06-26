using Cysharp.Threading.Tasks;
using QFramework;

namespace Core.SceneManagement
{
    public interface ISceneTransition : IUtility
    {
        UniTask FadeOut(float duration = 0.3f);

        UniTask FadeIn(float duration = 0.3f);

        void SetImmediate(bool black);
    }
}