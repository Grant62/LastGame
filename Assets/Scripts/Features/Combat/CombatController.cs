using System.Collections.Generic;
using System.Linq;
using Configuration.ExcelData.Container;
using Core.Architecture;
using Core.Systems;
using Features.Card.Command;
using Features.Card.Event;
using Features.Card.Interfaces;
using Features.Card.Model;
using Features.Card.System;
using Features.Card.UI;
using Features.Card.Utility;
using Features.Card.View;
using Features.Combat.Command;
using Features.Combat.Event;
using Features.Combat.UI;
using Features.Combat.Utility;
using Features.Combat.View.Board;
using Features.Enemy.Utility;
using Features.Enemy.View;
using Features.Hero.Command;
using Features.Hero.Define;
using Features.Hero.Event;
using Features.Hero.Model;
using Features.Hero.View;
using Features.Sword.Model;
using Features.Sword.View;
using Main.GM;
using QFramework;
using Services.ExcelTool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Combat
{
    public class CombatController : MonoBehaviour, IController
    {
        [BoxGroup("预制体")]
        [SerializeField] private BoardView board;
        [BoxGroup("预制体")]
        [SerializeField] private HeroView heroPrefab;
        [BoxGroup("预制体")]
        [SerializeField] private CardView cardUIPrefab;
        [BoxGroup("预制体")]
        [SerializeField] private GameObject arrowViewPrefab;
        [BoxGroup("预制体")]
        [SerializeField] private GameObject cursorViewPrefab;
        [BoxGroup("预制体")]
        [SerializeField] private SwordView swordPrefab;

        [BoxGroup("卡牌测试")]
        [SerializeField] private bool testMode;
        [BoxGroup("卡牌测试")]
        [SerializeField] private TextAsset testDeckJson;

        [BoxGroup("Overlay")]
        [SerializeField] private Canvas overlayCanvas;

        [BoxGroup("箭头偏移量")]
        [SerializeField] private float arrowOffset = 80f;

        [BoxGroup("GM")]
        [SerializeField] private GmPanel gmPanel;

        private HeroView mHeroUI;
        private readonly List<SwordView> mSpiritSwordViews = new();

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

            GameMain.Interface.RegisterUtility<IEnemyViewPool>(new EnemyViewPool(board.EnemyPrefab));

            GameMain.Interface.RegisterUtility<ICardViewPool>(new CardViewPool(cardUIPrefab));

            GameMain.Interface.RegisterUtility<ITargetResolver>(
                new EnemyTargetResolver(() => board.EnemyViews, this.GetSystem<IRandomSystem>()));

            GameMain.Interface.RegisterUtility<ITargetSelector>(new TargetSelector(mHeroUI));

            Transform overlayTrans = overlayCanvas.transform;

            CardView hoverCard = Instantiate(cardUIPrefab, overlayTrans);
            GameMain.Interface.RegisterUtility<ICardHoverDisplay>(
                new CardHoverDisplay(hoverCard));

            GameObject arrowView = Instantiate(arrowViewPrefab, overlayTrans);
            GameObject arrowHead = arrowView.transform.Find("Head").gameObject;
            GameObject arrowLine = arrowView.transform.Find("Line").gameObject;
            GameMain.Interface.RegisterUtility<IArrowDisplay>(
                new ArrowDisplay(arrowHead, arrowLine, arrowOffset));

            GameObject cursorView = Instantiate(cursorViewPrefab, overlayTrans);
            GameMain.Interface.RegisterUtility<ICursorDisplay>(new CursorDisplay(cursorView));

            EntryInfoContainer entryContainer = this.GetUtility<IBinaryDataMgr>().GetTable<EntryInfoContainer>();
            GameMain.Interface.RegisterUtility<IKeywordResolver>(new KeywordResolver(entryContainer));

            GameMain.Interface.RegisterUtility<ICardSpriteCache>(new CardSpriteCache());

            GameMain.Interface.SendEvent<GameReadyEvent>();
        }

        private void Start()
        {
            PositionHeroAtCenter();

            InitHero();
            InitSword();
            InitSpiritSwordTracking();
            this.SendCommand<InitEnemiesCommand>();
            InitDeck();

            this.RegisterEvent<PlayerMoveExecutedEvent>(OnPlayerMoved)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<HeroDeathEvent>(OnHeroDeath)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<EnemyDiedEvent>(OnEnemyDied)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<HandDiscardRequestEvent>(OnHandDiscardRequest)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            UIKit.OpenPanel<BattleBottomPanel>();

            this.SendCommand<StartBattleCommand>();
        }

        private void OnPlayerMoved(PlayerMoveExecutedEvent e)
        {
            board.ShiftEnemies(e.OldSlotIndex, e.NewSlotIndex);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                if (gmPanel.gameObject.activeSelf)
                    gmPanel.gameObject.SetActive(false);
                else
                {
                    gmPanel.gameObject.SetActive(true);
                    gmPanel.Open();
                }
            }
        }

        private void OnHeroDeath(HeroDeathEvent e)
        {
            this.SendCommand<SendBattleDefeatCommand>();
        }

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            if (!board.GetActiveEnemies().Any())
                this.SendCommand<SendBattleVictoryCommand>();
        }

        private void OnHandDiscardRequest(HandDiscardRequestEvent e)
        {
            DiscardSelectPanelData data = new()
            {
                HandCards = this.GetModel<ICardModel>().HandPile,
                OnSelected = cardData =>
                {
                    if (cardData != null)
                    {
                        this.GetSystem<ICardSystem>().RemoveFromHand(cardData);
                        this.GetSystem<ICardSystem>().AddToDiscard(cardData);
                    }
                }
            };

            UIKit.OpenPanel<DiscardSelectPanel>(UILevel.PopUI, data);
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
            Transform boardPanelParent = board.BoardPanel.parent;
            SwordView swordView = Instantiate(swordPrefab, boardPanelParent);
            swordView.transform.SetSiblingIndex(board.BoardPanel.GetSiblingIndex() + 1);
            swordView.Init(board);

            ISwordModel sword = this.GetModel<ISwordModel>();
            IHeroModel hero = this.GetModel<IHeroModel>();
            sword.CurSlotIndex.Value = hero.CurSlotIndex.Value;
        }

        private void InitSpiritSwordTracking()
        {
            ISwordModel sword = this.GetModel<ISwordModel>();
            sword.OnSpiritSwordsChanged.Register(SyncSpiritSwordViews)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void SyncSpiritSwordViews()
        {
            foreach (SwordView view in mSpiritSwordViews)
                Destroy(view.gameObject);
            mSpiritSwordViews.Clear();

            ISwordModel sword = this.GetModel<ISwordModel>();
            Transform boardPanelParent = board.BoardPanel.parent;
            int targetSiblingIndex = board.BoardPanel.GetSiblingIndex() + 1;
            foreach (int slotIndex in sword.SpiritSwordSlots)
            {
                SwordView spiritView = Instantiate(swordPrefab, boardPanelParent);
                spiritView.transform.SetSiblingIndex(targetSiblingIndex);
                spiritView.Init(board, false);
                spiritView.SetColor(Color.black);
                mSpiritSwordViews.Add(spiritView);

                spiritView.transform.position = board.GetSlotTransform(slotIndex).position
                                                + Vector3.up * swordPrefab.YOffset;
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
                EnemyView enemy = board.SpawnEnemy(slotIndex);
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

            this.SendCommand<LoadDeckFromExcelCommand>();
        }
    }
}