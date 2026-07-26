using Cysharp.Threading.Tasks;
using QFramework;

namespace Core.SceneManagement
{
    public interface ISceneManager : ISystem
    {
        SceneBase CurrentMainScene { get; }

        SceneBase CurrentRoomScene { get; }

        void SetRoomContainer(SceneContainer roomContainer);

        UniTask LoadMainScene(string sceneId, SceneLoadContext ctx = null);

        UniTask LoadRoomScene(string sceneId, SceneLoadContext ctx = null);
    }
}
