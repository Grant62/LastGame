using Core.Architecture;
using Core.SceneManagement.Define;
using Cysharp.Threading.Tasks;
using Features.Card.Command;
using Features.Card.Utility;
using Features.Configuration.Model;
using Features.Hero.Command;
using Features.Hero.Define;
using QFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.SceneManagement
{
    public class RunScene : SceneBase
    {
        public override string SceneId { get => "RunSceneRoot"; }

        public override SceneContainerType ContainerType { get => SceneContainerType.Main; }

        private SceneContainer mRoomContainer;

        public override async UniTask OnSceneEnter(SceneLoadContext ctx)
        {
            GameObject roomObj = new("RoomContainer");
            roomObj.transform.SetParent(transform, false);
            roomObj.transform.SetAsFirstSibling();
            mRoomContainer = roomObj.AddComponent<SceneContainer>();

            this.GetSystem<ISceneManager>().SetRoomContainer(mRoomContainer);

            await InitRunData();

            await this.GetSystem<ISceneManager>().LoadRoomScene("PreBattleRoomRoot");

            _ = Addressables.InstantiateAsync("TopBarPanel", GameRoot.CommonLayer);
        }

        private async UniTask InitRunData()
        {
            this.SendCommand<LoadCardDefinesCommand>();

            this.SendCommand(new SetupHeroCommand(new HeroDefine
            {
                MaxHealth = 100,
                InitialHealth = 80
            }));

            cfg.TbEntryInfo entryTable = this.GetUtility<ILubanDataModel>().Tables.TbEntryInfo;
            GameMain.Interface.RegisterUtility<IKeywordResolver>(new KeywordResolver(entryTable));
            GameMain.Interface.RegisterUtility<ICardSpriteCache>(new CardSpriteCache());

            AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>("TestDeck");
            await handle.Task;
            if (handle.Result != null)
                this.SendCommand(new InitDeckFromJsonCommand(handle.Result));
        }
    }
}