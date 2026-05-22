using Core.Architecture;
using Features.Card.Interfaces;
using Features.Card.Pool;
using Features.Card.View;
using Features.Combat.Command;
using Features.Combat.Targeting;
using Features.Combat.Targeting.View;
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

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            RegisterTargetingUtilities();
            RegisterCardViewPool();
        }

        private void Start()
        {
            InitHero();
            this.SendCommand<StartBattleCommand>();
        }

        private static void InitHero()
        {
            GameMain.Interface.SendCommand(new SetupHeroCommand(new HeroDefine
            {
                MaxHealth = 100
            }));
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
        }
    }
}