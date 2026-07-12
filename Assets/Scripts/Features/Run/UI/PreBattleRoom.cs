using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Core.Architecture;
using Core.SceneManagement;
using Core.SceneManagement.Define;
using Core.Systems;
using Cysharp.Threading.Tasks;
using Features.Card.Model;
using Features.Card.UI;
using Features.Card.Utility;
using Features.Card.View;
using Features.Combat.Model;
using Features.Hero.Command;
using Features.Hero.Model;
using Features.Run.Command;
using Features.Run.Data;
using Features.Run.Model;
using QFramework;
using Services.ExcelTool;
using TMPro;
using UnityEngine;

namespace Features.Run.UI
{
    public class PreBattleRoom : SceneBase, IController
    {
        public override string SceneId { get => "PreBattleRoomRoot"; }

        public override SceneContainerType ContainerType { get => SceneContainerType.Room; }

        [SerializeField] private RoomBox roomBox1;
        [SerializeField] private RoomBox roomBox2;
        [SerializeField] private RoomBox roomBox3;
        [SerializeField] private TMP_Text headerRestCountText;
        [SerializeField] private CardView cardViewPrefab;

        private RoomPreviewData[] mData;
        private int mLastLayer;
        private readonly Dictionary<string, EnemyGroupInfo> mInfoByLevelNum = new();
        private int mSkippedStep = -1;

        public new IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public override UniTask OnSceneEnter(SceneLoadContext ctx)
        {
            GameMain.Interface.RegisterUtility<ICardViewPool>(new CardViewPool(cardViewPrefab, transform));

            EnemyGroupInfoContainer table = this.GetUtility<IBinaryDataMgr>().GetTable<EnemyGroupInfoContainer>();
            mInfoByLevelNum.Clear();
            foreach (EnemyGroupInfo info in table.DataDic.Values)
                mInfoByLevelNum[info.LevelNum] = info;

            IRunModel run = this.GetModel<IRunModel>();
            int layer = run.CurrentLayer.Value;

            if (layer != mLastLayer && mLastLayer > 0)
                OnLayerChanged(mLastLayer);

            mLastLayer = layer;

            if (run.CurrentStep.Value == 3 && GetLevelType(layer, 3) == "Boss")
                AutoShortRest(run);

            BuildData();
            RenderAll();
            return UniTask.CompletedTask;
        }

        public override UniTask OnSceneExit()
        {
            this.GetUtility<ICardViewPool>().Dispose();
            return UniTask.CompletedTask;
        }

        private void OnLayerChanged(int prevLayer)
        {
            if (GetLevelType(prevLayer, 3) == "Boss")
                this.SendCommand<RestoreFullHealthCommand>();

            IGameConfigModel config = this.GetModel<IGameConfigModel>();
            this.SendCommand(new ResetShortRestCountCommand(config.ShortRestMaxCount));
        }

        private void AutoShortRest(IRunModel run)
        {
            int count = run.ShortRestCount.Value;
            if (count <= 0)
                return;

            IGameConfigModel config = this.GetModel<IGameConfigModel>();
            IHeroModel hero = this.GetModel<IHeroModel>();
            int heal = Mathf.CeilToInt(hero.MaxHealth.Value * config.ShortRestHealPercent);
            this.SendCommand(new HeroTakeHealCommand(heal * count));
            this.SendCommand(new ResetShortRestCountCommand(0));
        }

        private void BuildData()
        {
            IRunModel run = this.GetModel<IRunModel>();
            int layer = run.CurrentLayer.Value;
            int currentStep = run.CurrentStep.Value;
            int shortRestCount = run.ShortRestCount.Value;

            mData = new RoomPreviewData[3];
            for (int i = 0; i < 3; i++)
            {
                int step = i + 1;
                RoomBoxState state;
                if (step < currentStep)
                    state = step == mSkippedStep ? RoomBoxState.Skipped : RoomBoxState.Cleared;
                else if (step == currentStep)
                    state = RoomBoxState.Current;
                else
                    state = RoomBoxState.Upcoming;

                EnemyGroupInfo info = GetInfo(layer, step);
                string levelType = info?.LevelType ?? "Normal";
                string bossPreview = info?.Attribute ?? "";
                bool canShortRest = step != 3;

                mData[i] = new RoomPreviewData(layer, step, levelType, state,
                    canShortRest, shortRestCount, bossPreview);
            }

            headerRestCountText.text = $"剩余短休次数: {shortRestCount}";
        }

        private EnemyGroupInfo GetInfo(int layer, int step)
        {
            string levelNum = $"{layer}-{step}";
            mInfoByLevelNum.TryGetValue(levelNum, out EnemyGroupInfo info);
            return info;
        }

        private string GetLevelType(int layer, int step)
        {
            return GetInfo(layer, step)?.LevelType ?? "Normal";
        }

        private void RenderAll()
        {
            if (mData.Length > 0) roomBox1.Render(mData[0]);
            if (mData.Length > 1) roomBox2.Render(mData[1]);
            if (mData.Length > 2) roomBox3.Render(mData[2]);

            BindClicks();
        }

        private void BindClicks()
        {
            for (int i = 0; i < mData.Length && i < 3; i++)
            {
                if (mData[i].State != RoomBoxState.Current)
                    continue;

                int capturedIndex = i;
                RoomBox box = i switch
                {
                    0 => roomBox1,
                    1 => roomBox2,
                    _ => roomBox3
                };

                box.SetOnActionClick(() => OnEnterCombat(capturedIndex));
                box.SetOnShortRestClick(() => OnShortRest(capturedIndex));
            }
        }

        private void OnEnterCombat(int stepIndex)
        {
            OnEnterCombatAsync(stepIndex).Forget();
        }

        private async UniTaskVoid OnEnterCombatAsync(int stepIndex)
        {
            mSkippedStep = -1;
            string levelId = $"room_0{stepIndex + 1}";
            await this.GetSystem<ISceneManager>()
                .LoadRoomScene("CombatRoomRoot", new SceneLoadContext { LevelId = levelId });
        }

        private void OnShortRest(int stepIndex)
        {
            IRunModel run = this.GetModel<IRunModel>();
            mSkippedStep = run.CurrentStep.Value;
            this.SendCommand<ConsumeShortRestCommand>();

            IHeroModel hero = this.GetModel<IHeroModel>();
            IGameConfigModel config = this.GetModel<IGameConfigModel>();
            int heal = Mathf.CeilToInt(hero.MaxHealth.Value * config.ShortRestHealPercent);
            this.SendCommand(new HeroTakeHealCommand(heal));

            this.SendCommand<AdvanceStepCommand>();

            BuildData();
            RenderAll();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                PileGridPanel.ToggleDrawPile(this.GetModel<ICardModel>().Library);

            if (Input.GetKeyDown(KeyCode.Escape))
                this.GetSystem<IPopupStackSystem>().HandleEsc();
        }
    }
}