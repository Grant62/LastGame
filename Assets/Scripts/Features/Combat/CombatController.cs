using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Core.Architecture;
using DG.Tweening;
using Features.Card.Command;
using Features.Card.Data;
using Features.Card.Define;
using Features.Card.Event;
using Features.Card.Interfaces;
using Features.Card.Pool;
using Features.Card.System;
using Features.Card.View;
using Features.Combat.Command;
using Features.Combat.Event;
using Features.Combat.Targeting;
using Features.Combat.Targeting.View;
using Features.Enemy.View;
using Features.Hero;
using Features.Hero.Command;
using Features.Hero.View;
using Features.Sword.Command;
using Features.Sword.Model;
using Features.Sword.View;
using QFramework;
using Services.ExcelTool;
using Services.SlotDetector;
using UnityEngine;

namespace Features.Combat
{
    public class CombatController : MonoBehaviour, IController
    {
        [SerializeField] private HandView mHandView;
        [SerializeField] private CardView mCardPrefab;
        [SerializeField] private ArrowView mArrowView;
        [SerializeField] private GameObject mCursorPrefab;
        [SerializeField] private LayerMask mTargetLayer;
        [SerializeField] private BoardView mBoard;
        [SerializeField] private EnemyView mEnemyPrefab;
        [SerializeField] private SwordView mSwordPrefab;
        [SerializeField] private LayerMask slotLayer;
        [SerializeField] private bool mTestMode;
        [SerializeField] private TextAsset mTestDeckJson;

        private SwordView mSwordView;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            this.SendCommand<LoadCardDefinesCommand>();

            RegisterTargetingUtilities();
            RegisterCardViewPool();
            RegisterEnemyTargetResolver();
        }

        private void Start()
        {
            InitHero();
            InitEnemies();
            InitDeck();

            this.RegisterEvent<CardPlayedEvent>(OnCardPlayed)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<PlayerMoveExecutedEvent>(OnPlayerMoved)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.SendCommand<StartBattleCommand>();
        }

        private void OnPlayerMoved(PlayerMoveExecutedEvent e)
        {
            ISwordModel sword = this.GetModel<ISwordModel>();
            int swordShiftedTo = -1;
            Transform targetSlot = mBoard.GetSlotTransform(e.NewSlotIndex);
            HeroView heroView = FindFirstObjectByType<HeroView>();

            mBoard.ShiftEnemies(e.OldSlotIndex, e.NewSlotIndex,
                (oldSlot, newSlot) =>
                {
                    if (sword.IsSummoned.Value && sword.CurrentSlotIndex.Value == oldSlot)
                        swordShiftedTo = newSlot;
                });

            if (targetSlot != null && heroView != null)
            {
                heroView.transform
                    .DOMove(targetSlot.position, mBoard.MoveDuration)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        if (sword.IsSummoned.Value && sword.IsFollowingPlayer.Value)
                            sword.CurrentSlotIndex.Value = e.NewSlotIndex;
                        else if (swordShiftedTo >= 0)
                        {
                            sword.CurrentSlotIndex.Value = -1;
                            sword.CurrentSlotIndex.Value = swordShiftedTo;
                        }
                    });
            }
        }

        private void OnCardPlayed(CardPlayedEvent e)
        {
            if (e.CardData.CardId == 12001)
            {
                if (mSwordView == null && mSwordPrefab != null)
                {
                    mSwordView = Instantiate(mSwordPrefab);
                    mSwordView.Init(mBoard);
                }

                this.SendCommand<SummonSwordCommand>();
            }
        }

        private void InitHero()
        {
            this.SendCommand(new SetupHeroCommand(new HeroDefine
            {
                MaxHealth = 100,
                InitialHealth = 80
            }));

            this.SendCommand(new SetHeroSlotCommand(3));
        }

        private void InitEnemies()
        {
            int[] hpValues = { 40, 50, 60, 70, 80 };
            int[] dmgValues = { 5, 5, 6, 6, 7 };

            for (int i = 0; i < hpValues.Length; i++)
            {
                Transform slot = mBoard.GetFirstAvailableSlot();
                if (slot == null)
                    break;

                EnemyView enemyView = mBoard.SpawnEnemy(slot);
                enemyView.Init(1000 + i, hpValues[i], dmgValues[i]);
            }
        }

        private void InitDeck()
        {
            if (mTestMode && mTestDeckJson != null)
            {
                this.SendCommand(new InitDeckFromJsonCommand(mTestDeckJson));
                return;
            }

            IBinaryDataMgr dataMgr = this.GetUtility<IBinaryDataMgr>();
            CardInfoContainer cardContainer = dataMgr.GetTable<CardInfoContainer>();
            StartingCardInfoContainer startContainer = dataMgr.GetTable<StartingCardInfoContainer>();
            if (cardContainer == null || startContainer == null)
                return;

            Dictionary<string, int> nameToId = new();
            foreach (CardInfo info in cardContainer.DataDic.Values)
                nameToId[info.Name] = info.CardId;

            ICardDefineModel defines = this.GetModel<ICardDefineModel>();
            List<CardData> deck = new();

            foreach (StartingCardInfo start in startContainer.DataDic.Values)
            {
                if (nameToId.TryGetValue(start.CardName, out int cardId)
                    && defines.TryGet(cardId, out CardDefine define))
                {
                    deck.Add(define.CreateCardData());
                }
            }

            this.GetSystem<ICardSystem>().InitDrawPile(deck);
        }

        private void RegisterTargetingUtilities()
        {
            CursorDisplay cursorDisplay = new(mCursorPrefab);
            ArrowDisplay arrowDisplay = new(mArrowView);
            RaycastTargetSelector targetSelector = new(mTargetLayer);

            GameMain.Interface.RegisterUtility<ICursorDisplay>(cursorDisplay);
            GameMain.Interface.RegisterUtility<IArrowDisplay>(arrowDisplay);
            GameMain.Interface.RegisterUtility<ITargetSelector>(targetSelector);
            GameMain.Interface.RegisterUtility<ISlotDetector>(new SlotDetector(mBoard, slotLayer));
        }

        private void RegisterCardViewPool()
        {
            CardViewPool pool = new(mCardPrefab);
            GameMain.Interface.RegisterUtility<ICardViewPool>(pool);
            mHandView.SetCardViewPool(pool);
        }

        private void RegisterEnemyTargetResolver()
        {
            GameMain.Interface.RegisterUtility<ITargetResolver>(new EnemyTargetResolver());
        }
    }
}