using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Core.Architecture;
using Core.Systems;
using Features.Card;
using Features.Card.Command;
using Features.Card.Interfaces;
using Features.Card.Utility;
using Features.Card.View;
using Features.Combat.Command;
using Features.Combat.Event;
using Features.Combat.Targeting;
using Features.Combat.Utility;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Hero.Command;
using Features.Hero.Define;
using Features.Hero.Model;
using Features.Hero.View;
using Features.Sword.Model;
using Features.Sword.View;
using QFramework;
using Services.ExcelTool;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Combat
{
    public class CombatController : MonoBehaviour, IController
    {
        [SerializeField] private BoardView board;
        [SerializeField] private HeroView heroPrefab;
        [SerializeField] private CardView cardUIPrefab;
        [SerializeField] private bool testMode;
        [SerializeField] private TextAsset testDeckJson;

        [Header("Arrow")]
        [SerializeField] private GameObject arrowViewPrefab;
        [SerializeField] private float arrowOffset = 0.8f;

        [Header("Cursor")]
        [SerializeField] private GameObject cursorViewPrefab;

        [Header("Sword")]
        [SerializeField] private SwordView swordPrefab;

        [Header("Overlay")]
        [SerializeField] private Canvas overlayCanvas;

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

            this.SendCommand<StartBattleCommand>();
        }

        private void OnPlayerMoved(PlayerMoveExecutedEvent e)
        {
            board.ShiftEnemies(e.OldSlotIndex, e.NewSlotIndex);
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
            SwordView swordView = Instantiate(swordPrefab, transform);
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
            foreach (int slotIndex in sword.SpiritSwordSlots)
            {
                SwordView spiritView = Instantiate(swordPrefab, transform);
                spiritView.Init(board);
                spiritView.GetComponent<Image>().color = Color.black;
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