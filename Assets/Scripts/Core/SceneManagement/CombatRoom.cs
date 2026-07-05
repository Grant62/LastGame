using Core.Architecture;
using Core.SceneManagement.Define;
using Cysharp.Threading.Tasks;
using QFramework;

namespace Core.SceneManagement
{
    public class CombatRoom : SceneBase, IController
    {
        public override string SceneId { get => "CombatRoomRoot"; }

        public override SceneContainerType ContainerType { get => SceneContainerType.Room; }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public override UniTask OnSceneEnter(SceneLoadContext ctx)
        {
            return UniTask.CompletedTask;
        }
    }
}