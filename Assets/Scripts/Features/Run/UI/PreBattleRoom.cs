using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Core.Architecture;
using Core.SceneManagement;
using Core.SceneManagement.Define;
using Cysharp.Threading.Tasks;
using Features.Hero.Model;
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

        private RoomPreviewData[] mData;
        private int mLastLayer;
        private EnemyGroupInfoContainer mTable;

        public new IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public override UniTask OnSceneEnter(SceneLoadContext ctx)
        {
            mTable ??= this.GetUtility<IBinaryDataMgr>().GetTable<EnemyGroupInfoContainer>();

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

        private void OnLayerChanged(int prevLayer)
        {
            IRunModel run = this.GetModel<IRunModel>();

            if (GetLevelType(prevLayer, 3) == "Boss")
            {
                IHeroModel hero = this.GetModel<IHeroModel>();
                hero.Health.Value = hero.MaxHealth.Value;
            }

            run.ShortRestCount.Value = 2;
        }

        private void AutoShortRest(IRunModel run)
        {
            int count = run.ShortRestCount.Value;
            if (count <= 0)
                return;

            IHeroModel hero = this.GetModel<IHeroModel>();
            int heal = Mathf.CeilToInt(hero.MaxHealth.Value * 0.25f);
            hero.Health.Value = Mathf.Min(hero.Health.Value + heal * count, hero.MaxHealth.Value);
            run.ShortRestCount.Value = 0;
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
                    state = RoomBoxState.Cleared;
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
            foreach (KeyValuePair<int, EnemyGroupInfo> kv in mTable?.DataDic ?? new Dictionary<int, EnemyGroupInfo>())
            {
                if (kv.Value.LevelNum == levelNum)
                    return kv.Value;
            }

            return null;
        }

        private string GetLevelType(int layer, int step)
        {
            return GetInfo(layer, step)?.LevelType ?? "Normal";
        }

        private void RenderAll()
        {
            if (roomBox1 != null && mData.Length > 0) roomBox1.Render(mData[0]);
            if (roomBox2 != null && mData.Length > 1) roomBox2.Render(mData[1]);
            if (roomBox3 != null && mData.Length > 2) roomBox3.Render(mData[2]);

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

                if (box == null)
                    continue;

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
            string levelId = $"room_0{stepIndex + 1}";
            await this.GetSystem<ISceneManager>()
                .LoadRoomScene("CombatRoomRoot", new SceneLoadContext { LevelId = levelId });
        }

        private void OnShortRest(int stepIndex)
        {
            IRunModel run = this.GetModel<IRunModel>();
            run.ShortRestCount.Value--;

            IHeroModel hero = this.GetModel<IHeroModel>();
            int heal = Mathf.CeilToInt(hero.MaxHealth.Value * 0.25f);
            hero.Health.Value = Mathf.Min(hero.Health.Value + heal, hero.MaxHealth.Value);

            run.CurrentStep.Value++;

            BuildData();
            RenderAll();
        }
    }
}