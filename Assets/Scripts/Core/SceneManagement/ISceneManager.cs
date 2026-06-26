using Cysharp.Threading.Tasks;
using QFramework;

namespace Core.SceneManagement
{
    public interface ISceneManager : ISystem
    {
        SceneBase CurrentMainScene { get; }

        SceneBase CurrentRoomScene { get; }

        UniTask LoadMainScene(string sceneId, SceneLoadContext ctx = null, bool withTransition = false);

        UniTask LoadRoomScene(string sceneId, SceneLoadContext ctx = null);

        UniTask ShowOverlay(string overlayId, SceneLoadContext ctx = null);

        UniTask HideOverlay();

        UniTask PreloadScene(string sceneId);
    }
}