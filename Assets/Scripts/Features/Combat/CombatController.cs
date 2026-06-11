using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Core.Architecture;
using Core.Systems;
using Features.Card.Command;
using Features.Card.Data;
using Features.Card.Define;
using Features.Card.Interfaces;
using Features.Card.Pool;
using Features.Card.System;
using Features.Card.UI;
using Features.Combat.Command;
using Features.Combat.Event;
using Features.Combat.Targeting;
using Features.Combat.UI;
using Features.Combat.UI.Board;
using Features.Hero.Command;
using Features.Hero.Define;
using Features.Hero.Model;
using Features.Hero.View;
using Features.Sword.Command;
using Features.Sword.Model;
using Features.Sword.UI;
using QFramework;
using Services.ExcelTool;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Combat
{
    public class CombatController : MonoBehaviour, IController
    {
        [SerializeField] private BoardPanel board;
        [SerializeField] private HeroView heroPrefab;
        [SerializeField] private CardUI cardUIPrefab;
        [SerializeField] private bool testMode;
        [SerializeField] private TextAsset testDeckJson;

        [Header("Arrow")]
        [SerializeField] private GameObject arrowViewPrefab;
        [SerializeField] private float arrowOffset = 0.8f;

        [Header("Cursor")]
        [SerializeField] private GameObject cursorViewPrefab;

        [Header("Hover")]
        [SerializeField] private float cardHoverOffset = 150f;

        [Header("Sword")]
        [SerializeField] private SwordUI swordPrefab;

        [Header("Overlay")]
        [SerializeField] private Canvas overlayCanvas;

        private HeroView mHeroUI;
        private readonly List<SwordUI> mSpiritSwordViews = new();

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            this.SendCommand<LoadCardDefinesCommand>();

            mHeroUI = Instantiate(heroPrefab, transform);

            RegisterUtilities();
        }

        private void RegisterUtilities()
        {
            GameMain.Interface.RegisterUtility<IBoardAccess>(new BoardAccess(board));

            GameMain.Interface.RegisterUtility<ICardUIPool>(new CardUIPool(cardUIPrefab));

            GameMain.Interface.RegisterUtility<ITargetResolver>(
                new EnemyTargetResolver(() => board.EnemyViews, this.GetSystem<IRandomSystem>()));

            GameMain.Interface.RegisterUtility<ITargetSelector>(new TargetSelector(mHeroUI));

            Transform overlayTrans = overlayCanvas != null ? overlayCanvas.transform : transform;

            CardUI hoverCard = Instantiate(cardUIPrefab, overlayTrans);
            GameMain.Interface.RegisterUtility<ICardHoverDisplay>(
                new CardHoverDisplay(hoverCard, cardHoverOffset));

            GameObject arrowView = Instantiate(arrowViewPrefab, overlayTrans);
            GameObject arrowHead = arrowView.transform.Find("Head").gameObject;
            GameObject arrowLine = arrowView.transform.Find("Line").gameObject;
            GameMain.Interface.RegisterUtility<IArrowDisplay>(
                new ArrowDisplay(arrowHead, arrowLine, arrowOffset));

            GameObject cursorView = Instantiate(cursorViewPrefab, overlayTrans);
            GameMain.Interface.RegisterUtility<ICursorDisplay>(new CursorDisplay(cursorView));

            GameMain.Interface.SendEvent<GameReadyEvent>();
        }

        private void Start()
        {
            PositionHeroAtCenter();

            InitHero();
            InitSword();
            InitSpiritSwordTracking();
            InitEnemies();
            InitDeck();

            this.RegisterEvent<PlayerMoveExecutedEvent>(OnPlayerMoved)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.SendCommand<StartBattleCommand>();
        }

        private void OnPlayerMoved(PlayerMoveExecutedEvent e)
        {
            ISwordModel sword = this.GetModel<ISwordModel>();
            int swordShiftedTo = -1;

            board.ShiftEnemies(e.OldSlotIndex, e.NewSlotIndex,
                (oldSlot, newSlot) =>
                {
                    if (sword.IsSummoned.Value && sword.CurSlotIndex.Value == oldSlot)
                        swordShiftedTo = newSlot;
                },
                () =>
                {
                    if (swordShiftedTo >= 0)
                    {
                        this.SendCommand(new UpdateSwordSlotCommand(-1));
                        this.SendCommand(new UpdateSwordSlotCommand(swordShiftedTo));
                    }
                });
        }

        private void InitHero()
        {
            this.SendCommand(new SetupHeroCommand(new HeroDefine
            {
                MaxHealth = 100,
                InitialHealth = 80
            }));

            this.SendCommand(new SetHeroSlotCommand(4));
        }

        private void PositionHeroAtCenter()
        {
            mHeroUI.transform.SetParent(board.GetSlotTransform(4));
            mHeroUI.transform.localPosition = Vector3.zero;
        }

        private void InitSword()
        {
            SwordUI swordUI = Instantiate(swordPrefab, transform);
            swordUI.Init(board);

            ISwordModel sword = this.GetModel<ISwordModel>();
            IHeroModel hero = this.GetModel<IHeroModel>();
            sword.CurSlotIndex.Value = hero.CurSlotIndex.Value;
            sword.IsSummoned.Value = true;
        }

        private void InitSpiritSwordTracking()
        {
            ISwordModel sword = this.GetModel<ISwordModel>();
            sword.OnSpiritSwordsChanged.Register(SyncSpiritSwordViews)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void SyncSpiritSwordViews()
        {
            foreach (SwordUI view in mSpiritSwordViews)
                Destroy(view.gameObject);
            mSpiritSwordViews.Clear();

            ISwordModel sword = this.GetModel<ISwordModel>();
            foreach (int slotIndex in sword.SpiritSwordSlots)
            {
                SwordUI spiritView = Instantiate(swordPrefab, transform);
                spiritView.Init(board);
                spiritView.GetComponent<Image>().color = Color.black;
                mSpiritSwordViews.Add(spiritView);

                spiritView.transform.position = board.GetSlotTransform(slotIndex).position;
            }
        }

        private void InitEnemies()
        {
            const int centerSlot = 4;
            int[] hpValues = { 40, 50, 60, 70, 80 };
            int[] dmgValues = { 5, 5, 6, 6, 7 };
            int totalSlots = 9;
            int sideCount = totalSlots / 2;

            int[] spawnOrder = new int[sideCount * 2];
            for (int i = 0; i < sideCount; i++)
            {
                spawnOrder[i * 2] = centerSlot - (i + 1);
                spawnOrder[i * 2 + 1] = centerSlot + i + 1;
            }

            for (int i = 0; i < hpValues.Length; i++)
            {
                int slotIndex = spawnOrder[i];
                EnemyUI enemy = board.SpawnEnemy(slotIndex);
                enemy.Init(1000 + i, hpValues[i], dmgValues[i]);
            }
        }

        private void InitDeck()
        {
            if (testMode && testDeckJson != null)
            {
                this.SendCommand(new InitDeckFromJsonCommand(testDeckJson));
                return;
            }

            LoadDeckFromExcel();
        }

        private void LoadDeckFromExcel()
        {
            IBinaryDataMgr dataMgr = this.GetUtility<IBinaryDataMgr>();
            CardInfoContainer cardContainer = dataMgr.GetTable<CardInfoContainer>();
            StartingCardInfoContainer startContainer = dataMgr.GetTable<StartingCardInfoContainer>();
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

            this.GetSystem<ICardSystem>().InitLibrary(deck);
        }
    }
}