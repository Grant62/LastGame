using Configuration.ExcelData.Container;
using Core.Architecture;
using Core.SceneManagement.Define;
using Cysharp.Threading.Tasks;
using Features.Card.Command;
using Features.Card.Interfaces;
using Features.Card.Utility;
using Features.Card.View;
using Features.Hero.Command;
using Features.Hero.Define;
using QFramework;
using Services.ExcelTool;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.SceneManagement
{
    public class RunScene : SceneBase, IController
    {
        public override string SceneId { get => "RunSceneRoot"; }

        public override SceneContainerType ContainerType { get => SceneContainerType.Main; }

        private SceneContainer mRoomContainer;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public override async UniTask OnSceneEnter(SceneLoadContext ctx)
        {
            GameObject roomObj = new("RoomContainer");
            roomObj.transform.SetParent(transform, false);
            roomObj.transform.SetAsFirstSibling();
            mRoomContainer = roomObj.AddComponent<SceneContainer>();

            this.GetSystem<ISceneManager>().SetRoomContainer(mRoomContainer);

            InitRunData();

            await this.GetSystem<ISceneManager>().LoadRoomScene("PreBattleRoomRoot");

            Addressables.InstantiateAsync("TopBarPanel", GameRoot.CommonLayer);
        }

        private void InitRunData()
        {
            this.SendCommand<LoadCardDefinesCommand>();

            this.SendCommand(new SetupHeroCommand(new HeroDefine
            {
                MaxHealth = 100,
                InitialHealth = 80
            }));

            EntryInfoContainer entryContainer = this.GetUtility<IBinaryDataMgr>().GetTable<EntryInfoContainer>();
            GameMain.Interface.RegisterUtility<IKeywordResolver>(new KeywordResolver(entryContainer));
            GameMain.Interface.RegisterUtility<ICardSpriteCache>(new CardSpriteCache());

            AsyncOperationHandle<GameObject> cardPrefabHandle = Addressables.LoadAssetAsync<GameObject>("CardView");
            cardPrefabHandle.WaitForCompletion();
            CardView cardPrefab = cardPrefabHandle.Result != null
                ? cardPrefabHandle.Result.GetComponent<CardView>()
                : null;
            GameMain.Interface.RegisterUtility<ICardViewPool>(new CardViewPool(cardPrefab));

            AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>("TestDeck");
            handle.WaitForCompletion();
            if (handle.Result != null)
                this.SendCommand(new InitDeckFromJsonCommand(handle.Result));
        }
    }
}