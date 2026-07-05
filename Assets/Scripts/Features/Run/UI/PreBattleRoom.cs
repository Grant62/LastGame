using Core.Architecture;
using Core.SceneManagement;
using Core.SceneManagement.Define;
using Cysharp.Threading.Tasks;
using Features.Run.Data;
using Features.Run.Model;
using QFramework;
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

        public new IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public override async UniTask OnSceneEnter(SceneLoadContext ctx)
        {
            BuildData();
            RenderAll();
            await UniTask.CompletedTask;
        }

        private void BuildData()
        {
            IRunModel run = this.GetModel<IRunModel>();
            int layer = run.CurrentLayer.Value;
            int currentStep = run.CurrentStep.Value;
            int shortRestCount = 1;

            string[] stepTypes = { "Normal", "Elite", "Boss" };

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

                bool canShortRest = step != 3;

                mData[i] = new RoomPreviewData(layer, step, stepTypes[i], state,
                    canShortRest, shortRestCount);
            }

            headerRestCountText.text = $"剩余短休次数: {shortRestCount}";
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
            if (stepIndex >= 0 && stepIndex < mData.Length)
            {
                mData[stepIndex] = new RoomPreviewData(
                    mData[stepIndex].Layer,
                    mData[stepIndex].Step,
                    mData[stepIndex].StepTypeText,
                    RoomBoxState.Rested,
                    mData[stepIndex].CanShortRest,
                    0,
                    mData[stepIndex].BossPreview
                );
            }

            headerRestCountText.text = "剩余短休次数: 0";
            RenderAll();
        }
    }
}