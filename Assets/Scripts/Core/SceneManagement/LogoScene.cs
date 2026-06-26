using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;

namespace Core.SceneManagement
{
    public class LogoScene : SceneBase
    {
        public override string SceneId { get => "LogoSceneRoot"; }

        public override SceneContainerType ContainerType { get => SceneContainerType.Main; }

        private bool mClicked;

        private void Update()
        {
            if (!mClicked && Input.GetMouseButtonDown(0))
            {
                mClicked = true;
                GoToMainMenu().Forget();
            }
        }

        private async UniTaskVoid GoToMainMenu()
        {
            await this.GetSystem<ISceneManager>().LoadMainScene("MainMenuSceneRoot");
        }
    }
}