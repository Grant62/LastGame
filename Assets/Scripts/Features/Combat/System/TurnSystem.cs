using System;
using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Cysharp.Threading.Tasks;
using Features.Combat.Event;
using Features.Combat.Utility;
using Features.Combat.View.Board;
using Features.Enemy.Data;
using Features.Enemy.Define;
using Features.Enemy.Event;
using Features.Enemy.Model;
using Features.Enemy.System;
using Features.Enemy.View;
using Features.Hero.Model;
using Features.Run.Model;
using QFramework;
using Services.ExcelTool;

namespace Features.Combat.System
{
    public class TurnSystem : AbstractSystem, ITurnSystem
    {
        private const int LeftSpawnSlot = 0;
        private const int RightSpawnSlot = 8;
        private const int MaxLayer = 8;

        private Dictionary<string, int[]> mLevelQueueCache;

        public int TurnCount { get; private set; }
        public bool IsPlayerTurn { get; private set; }

        protected override void OnInit() { }

        public void StartBattle()
        {
            TurnCount = 0;
            this.SendEvent<BattleStartEvent>();

            IEnemyModel enemyModel = this.GetModel<IEnemyModel>();
            IEnemyAISystem aiSystem = this.GetSystem<IEnemyAISystem>();

            RefreshEnemyQueue(enemyModel);

            FillSpawnSlots(enemyModel);

            ShowIntents(aiSystem, enemyModel);
            StartPlayerTurn();
        }

        public void EndPlayerTurn()
        {
            if (!IsPlayerTurn)
                return;

            IsPlayerTurn = false;
            this.SendEvent<PlayerTurnEndEvent>();
            StartEnemyTurn().Forget();
        }

        private void StartPlayerTurn()
        {
            TurnCount++;
            IsPlayerTurn = true;
            this.SendEvent<PlayerTurnStartEvent>();
        }

        private async UniTaskVoid StartEnemyTurn()
        {
            this.SendEvent<EnemyTurnStartEvent>();

            IEnemyModel enemyModel = this.GetModel<IEnemyModel>();
            IEnemyAISystem aiSystem = this.GetSystem<IEnemyAISystem>();
            int heroSlot = this.GetModel<IHeroModel>().CurSlotIndex.Value;

            if (enemyModel.AnyAlive)
            {
                aiSystem.CalculateIntents(heroSlot);
                await aiSystem.MovePhase(heroSlot);
            }

            FillSpawnSlots(enemyModel);

            if (this.GetModel<IHeroModel>().Health.Value <= 0)
            {
                this.SendEvent<EnemyTurnEndEvent>();
                return;
            }

            if (enemyModel.AnyAlive)
            {
                aiSystem.CalculateIntents(heroSlot);
                await aiSystem.AttackPhase(heroSlot);
            }

            if (this.GetModel<IHeroModel>().Health.Value <= 0)
            {
                this.SendEvent<EnemyTurnEndEvent>();
                return;
            }

            HandleStepComplete(enemyModel);

            this.SendEvent<EnemyTurnEndEvent>();

            ShowIntents(aiSystem, enemyModel);
            StartPlayerTurn();
        }

        private void FillSpawnSlots(IEnemyModel enemyModel)
        {
            BoardView board = this.GetUtility<IBoardAccess>().Board;

            if (board.GetEnemyAtSlot(LeftSpawnSlot) == null && enemyModel.HasMoreInQueue)
            {
                SpawnOne(enemyModel, board, enemyModel.EnemyIdQueue[enemyModel.QueueIndex], LeftSpawnSlot);
                enemyModel.QueueIndex++;
            }

            if (board.GetEnemyAtSlot(RightSpawnSlot) == null && enemyModel.HasMoreInQueue)
            {
                SpawnOne(enemyModel, board, enemyModel.EnemyIdQueue[enemyModel.QueueIndex], RightSpawnSlot);
                enemyModel.QueueIndex++;
            }
        }

        private void HandleStepComplete(IEnemyModel enemyModel)
        {
            if (!enemyModel.IsStepComplete)
                return;

            AdvanceStep(enemyModel);

            IRunModel run = this.GetModel<IRunModel>();
            if (run.CurrentLayer.Value > MaxLayer)
                return;

            this.SendEvent(new FloorClearedEvent
            {
                Layer = run.CurrentLayer.Value,
                Step = run.CurrentStep.Value
            });
        }

        private void AdvanceStep(IEnemyModel enemyModel)
        {
            this.GetUtility<IBoardAccess>().Board.ClearAllEnemies();
            enemyModel.ClearAll();

            IRunModel run = this.GetModel<IRunModel>();
            run.CurrentStep.Value++;
            if (run.CurrentStep.Value > 3)
            {
                run.CurrentStep.Value = 1;
                run.CurrentLayer.Value++;
            }

            if (run.CurrentLayer.Value > MaxLayer)
            {
                this.SendEvent<BattleVictoryEvent>();
                return;
            }

            RefreshEnemyQueue(enemyModel);
        }

        private void SpawnOne(IEnemyModel enemyModel, BoardView board, int monsterId, int slot)
        {
            EnemyView enemyView = board.SpawnEnemy(slot);

            int heroSlot = this.GetModel<IHeroModel>().CurSlotIndex.Value;
            IEnemyDefineModel defineModel = this.GetModel<IEnemyDefineModel>();
            bool hasDefine = defineModel.TryGet(monsterId, out EnemyDefine define);
            int maxHp = hasDefine ? define.MaxHealth : 40;
            int damage = hasDefine ? define.Damage : 6;

            EnemyRuntimeData data = new()
            {
                EnemyId = monsterId,
                HP = maxHp,
                MaxHP = maxHp,
                Armor = 0,
                Damage = damage,
                MoveSpeed = 1,
                SlotIndex = slot,
                IsFacingRight = slot < heroSlot,
                CurrentIntent = EnemyIntentType.Move
            };

            enemyView.Init(monsterId, data.MaxHP, data.Damage);
            enemyView.SetFacing(data.IsFacingRight);
            enemyModel.AddEnemy(data);
        }

        private void ShowIntents(IEnemyAISystem aiSystem, IEnemyModel enemyModel)
        {
            int heroSlot = this.GetModel<IHeroModel>().CurSlotIndex.Value;
            aiSystem.CalculateIntents(heroSlot);

            BoardView board = this.GetUtility<IBoardAccess>().Board;

            foreach (KeyValuePair<int, EnemyIntentType> kvp in enemyModel.CachedIntents)
            {
                if (kvp.Value == EnemyIntentType.None)
                    continue;

                EnemyView view = board.GetEnemyAtSlot(kvp.Key);
                if (view != null)
                    view.ShowIntent(kvp.Value);

                this.SendEvent(new EnemyIntentEvent { SlotIndex = kvp.Key, Intent = kvp.Value });
            }
        }

        private void RefreshEnemyQueue(IEnemyModel enemyModel)
        {
            if (mLevelQueueCache == null)
                BuildLevelQueueCache();

            IRunModel run = this.GetModel<IRunModel>();
            string levelNum = $"{run.CurrentLayer.Value}-{run.CurrentStep.Value}";

            mLevelQueueCache.TryGetValue(levelNum, out int[] queue);
            enemyModel.EnemyIdQueue = queue ?? Array.Empty<int>();
            enemyModel.QueueIndex = 0;
        }

        private void BuildLevelQueueCache()
        {
            mLevelQueueCache = new Dictionary<string, int[]>();

            EnemyGroupInfoContainer container = this.GetUtility<IBinaryDataMgr>()
                .GetTable<EnemyGroupInfoContainer>();
            if (container == null)
                return;

            foreach (EnemyGroupInfo info in container.DataDic.Values)
            {
                string[] parts = info.Content.Split(',');
                int[] queue = new int[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                    int.TryParse(parts[i], out queue[i]);

                mLevelQueueCache[info.LevelNum] = queue;
            }
        }
    }
}