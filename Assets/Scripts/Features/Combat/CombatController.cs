using System.Collections.Generic;
using Core.Architecture;
using Core.SceneManagement;
using Core.SceneManagement.Event;
using Core.Systems;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
using Features.Combat.Utility;
using Features.Combat.View.Board;
using Features.Enemy.Command;
using Features.Enemy.Utility;
using Features.Hero.Command;
using Features.Hero.Event;
using Features.Hero.Model;
using Features.Hero.View;
using Features.Sword.Model;
using Features.Sword.View;
using Presentation.Effects;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
        [SerializeField] private float cursorHeightOffset = 275f;
        [BoxGroup("预制体")]
        [SerializeField] private SwordView swordPrefab;
        [BoxGroup("预制体")]
        [SerializeField] private DamageTextUI damageTextPrefab;

        [BoxGroup("卡牌测试")]
        [SerializeField] private bool testMode;
        [BoxGroup("卡牌测试")]
        [SerializeField] private TextAsset testDeckJson;

        [BoxGroup("GM")]
        [SerializeField] private bool invincibleMode;

        private HeroView mHeroUI;
        private readonly List<SwordView> mSpiritSwordViews = new();

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            mHeroUI = Instantiate(heroPrefab, transform);

            RegisterUtilities();
        }

        private void RegisterUtilities()
        {
            GameMain.Interface.RegisterUtility<IBoardAccess>(new BoardAccess(board));

            GameMain.Interface.RegisterUtility<IEnemyViewPool>(new EnemyViewPool(board.EnemyPrefab));

            GameMain.Interface.RegisterUtility<ITargetResolver>(
                new EnemyTargetResolver(() => board.EnemyViews, this.GetSystem<IRandomSystem>()));

            GameMain.Interface.RegisterUtility<ITargetSelector>(new TargetSelector(mHeroUI));

            Transform overlayTrans = GameRoot.CombatOverlay;

            CardView hoverCard = Instantiate(cardUIPrefab, overlayTrans);
            GameMain.Interface.RegisterUtility<ICardHoverDisplay>(
                new CardHoverDisplay(hoverCard, this.GetUtility<IKeywordResolver>()));

            GameObject arrowView = Instantiate(arrowViewPrefab, overlayTrans);
            GameObject arrowHead = arrowView.transform.Find("Head").gameObject;
            GameObject arrowLine = arrowView.transform.Find("Line").gameObject;
            GameMain.Interface.RegisterUtility<IArrowDisplay>(
                new ArrowDisplay(arrowHead, arrowLine));

            GameObject cursorView = Instantiate(cursorViewPrefab, overlayTrans);
            GameMain.Interface.RegisterUtility<ICursorDisplay>(new CursorDisplay(cursorView, cursorHeightOffset));

            GameMain.Interface.RegisterUtility<IDamageTextSpawner>(
                new DamageTextSpawner(damageTextPrefab, overlayTrans));

            GameMain.Interface.SendEvent(new RoomReadyEvent { RoomId = "CombatRoom" });
        }

        private void Start()
        {
            PositionHeroAtCenter();

            InitHero();
            InitSword();
            InitSpiritSwordTracking();

            this.SendCommand<LoadEnemyDefinesCommand>();

            InitDeck();

            this.RegisterEvent<PlayerMoveExecutedEvent>(OnPlayerMoved)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<HeroDeathEvent>(OnHeroDeath)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<HandDiscardRequestEvent>(OnHandDiscardRequest)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<BattleVictoryEvent>(_ => OnBattleEnd())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<BattleDefeatEvent>(_ => OnBattleEnd())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<FloorClearedEvent>(OnFloorCleared)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            LoadBattleBottomPanel();

            this.SendCommand<StartBattleCommand>();
        }

        private async void LoadBattleBottomPanel()
        {
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(
                "BattleBottomPanel", GameRoot.CommonLayer);
            await handle.Task;
        }

        private void OnPlayerMoved(PlayerMoveExecutedEvent @event)
        {
            board.ShiftEnemies(@event.OldSlotIndex, @event.NewSlotIndex);
        }

        private void OnHeroDeath(HeroDeathEvent @event)
        {
            this.SendCommand<SendBattleDefeatCommand>();
        }

        private void OnBattleEnd()
        {
            this.GetUtility<IDamageTextSpawner>().ClearAll();
            GameMain.Interface.SendEvent<BattleEndCleanupEvent>();
        }

        private void OnFloorCleared(FloorClearedEvent @event)
        {
            this.GetSystem<ISceneManager>()
                .LoadRoomScene("PreBattleRoomRoot")
                .Forget();
        }

        private void OnHandDiscardRequest(HandDiscardRequestEvent @event)
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

            LoadDiscardSelectPanel(data);
        }

        private async void LoadDiscardSelectPanel(DiscardSelectPanelData data)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(
                "DiscardSelectPanel", GameRoot.PopUILayer);
            GameObject instance = await handle.Task;
            DiscardSelectPanel panel = instance.GetComponent<DiscardSelectPanel>();
            panel.Open(data);
        }

        private void InitHero()
        {
            if (invincibleMode)
                this.GetModel<IHeroModel>().Invincible.Value = true;

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

        private void InitDeck()
        {
            if (this.GetModel<ICardModel>().Library.Count > 0)
                return;

            if (testMode && testDeckJson != null)
            {
                this.SendCommand(new InitDeckFromJsonCommand(testDeckJson));
                return;
            }

            this.SendCommand<LoadDeckFromExcelCommand>();
        }

        private void SyncSpiritSwordViews()
        {
            ISwordModel sword = this.GetModel<ISwordModel>();

            if (sword.IsRecalling)
            {
                sword.IsRecalling = false;
                int targetSlot = sword.RecallTargetSlot;
                Vector3 targetPos = board.GetSlotTransform(targetSlot).position;

                foreach (SwordView view in mSpiritSwordViews)
                {
                    SwordView captured = view;
                    captured.transform.DOMoveX(targetPos.x, 0.4f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() => Destroy(captured.gameObject));
                }
            }
            else
            {
                foreach (SwordView view in mSpiritSwordViews)
                    Destroy(view.gameObject);
            }

            mSpiritSwordViews.Clear();

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
    }
}