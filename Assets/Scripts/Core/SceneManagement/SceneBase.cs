using Core.Architecture;
using Core.SceneManagement.Define;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;

namespace Core.SceneManagement
{
    public abstract class SceneBase : MonoBehaviour, IController
    {
        public abstract string SceneId { get; }

        public abstract SceneContainerType ContainerType { get; }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public virtual UniTask OnSceneEnter(SceneLoadContext ctx)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnSceneExit()
        {
            return UniTask.CompletedTask;
        }

        public virtual void OnScenePause() { }

        public virtual void OnSceneResume() { }
    }
}