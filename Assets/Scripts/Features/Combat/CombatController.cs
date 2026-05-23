using System.Collections.Generic;
using Core.Architecture;
using Features.Card.Command;
using Features.Card.Data;
using Features.Card.Effects;
using Features.Card.Interfaces;
using Features.Card.Pool;
using Features.Card.System;
using Features.Card.View;
using Features.Combat.Command;
using Features.Combat.Targeting;
using Features.Combat.Targeting.View;
using Features.Enemy.View;
using Features.Hero;
using Features.Hero.Command;
using QFramework;
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
        [SerializeField] private EnemyBoardView mEnemyBoard;
        [SerializeField] private EnemyView mEnemyPrefab;
        [SerializeField] private bool mTestMode;
        [SerializeField] private TextAsset mTestDeckJson;

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
            this.SendCommand<StartBattleCommand>();
        }

        private static void InitHero()
        {
            GameMain.Interface.SendCommand(new SetupHeroCommand(new HeroDefine
            {
                MaxHealth = 100
            }));
        }

        private void InitEnemies()
        {
            int[] hpValues = { 40, 50, 60 };

            for (int i = 0; i < hpValues.Length; i++)
            {
                Transform slot = mEnemyBoard.GetFirstAvailableSlot();
                if (slot == null)
                    continue;

                EnemyView enemyView = mEnemyBoard.SpawnEnemy(slot);
                enemyView.Init(1000 + i, hpValues[i], 5);
            }
        }

        private void InitDeck()
        {
            if (mTestMode && mTestDeckJson != null)
            {
                this.SendCommand(new InitDeckFromJsonCommand(mTestDeckJson));
            }
            else
            {
                List<CardData> cards = CardFactory.CreateStarterDeck();
                this.GetSystem<ICardSystem>().InitDrawPile(cards);
            }
        }

        private void RegisterTargetingUtilities()
        {
            CursorDisplay cursorDisplay = new(mCursorPrefab);
            ArrowDisplay arrowDisplay = new(mArrowView);
            RaycastTargetSelector targetSelector = new(mTargetLayer);

            GameMain.Interface.RegisterUtility<ICursorDisplay>(cursorDisplay);
            GameMain.Interface.RegisterUtility<IArrowDisplay>(arrowDisplay);
            GameMain.Interface.RegisterUtility<ITargetSelector>(targetSelector);
        }

        private void RegisterCardViewPool()
        {
            CardViewPool pool = new(mCardPrefab);
            GameMain.Interface.RegisterUtility<ICardViewPool>(pool);
            mHandView.SetCardViewPool(pool);
        }

        private static void RegisterEnemyTargetResolver()
        {
            GameMain.Interface.RegisterUtility<ITargetResolver>(new EnemyTargetResolver());
        }
    }
}