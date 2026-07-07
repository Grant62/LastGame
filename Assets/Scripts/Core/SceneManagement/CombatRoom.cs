using Core.Architecture;
using Core.SceneManagement.Define;
using Cysharp.Threading.Tasks;
using Features.Combat.Command;
using Features.Combat.Utility;
using Features.Enemy.Utility;
using UnityEngine;

namespace Core.SceneManagement
{
    public class CombatRoom : SceneBase
    {
        public override string SceneId { get => "CombatRoomRoot"; }

        public override SceneContainerType ContainerType { get => SceneContainerType.Room; }

        public override UniTask OnSceneEnter(SceneLoadContext ctx)
        {
            return UniTask.CompletedTask;
        }

        public override UniTask OnSceneExit()
        {
            GameMain.Interface.SendCommand(new ShuffleAllToDrawPileCommand());

            foreach (Transform child in GameRoot.CombatOverlay)
                Object.Destroy(child.gameObject);

            GameMain.Interface.GetUtility<IEnemyViewPool>().Dispose();
            GameMain.Interface.GetUtility<IDamageTextSpawner>().Dispose();

            return UniTask.CompletedTask;
        }
    }
}
