using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Core.SceneManagement
{
    public class MainMenuScene : SceneBase
    {
        public override string SceneId { get => "MainMenuSceneRoot"; }

        public override SceneContainerType ContainerType { get => SceneContainerType.Main; }

        [SerializeField] private Button startButton;

        private void Start()
        {
            if (startButton != null)
                startButton.onClick.AddListener(() => OnStartGame().Forget());
        }

        private async UniTaskVoid OnStartGame()
        {
            await this.GetSystem<ISceneManager>()
                .LoadMainScene("CombatRoomRoot", withTransition: true);
        }
    }
}