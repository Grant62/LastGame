using Features.Combat.Event;
using Features.Hero.Model;
using QFramework;

namespace Features.Combat.System
{
    public class InteractionSystem : AbstractSystem, IInteractionSystem
    {
        private int mAnimCounter;
        private bool mBattleEnded;

        public bool IsDragging { get; private set; }

        private bool IsAnimating { get => mAnimCounter > 0; }

        protected override void OnInit()
        {
            this.RegisterEvent<BattleVictoryEvent>(_ => mBattleEnded = true);
            this.RegisterEvent<BattleDefeatEvent>(_ => mBattleEnded = true);
            this.RegisterEvent<BattleStartEvent>(_ => mBattleEnded = false);
        }

        public void BeginDrag()
        {
            IsDragging = true;
        }

        public void EndDrag()
        {
            IsDragging = false;
        }

        public void BeginAnimation()
        {
            mAnimCounter++;
        }

        public void EndAnimation()
        {
            if (mAnimCounter > 0)
                mAnimCounter--;
        }

        public bool CanInteract()
        {
            if (mBattleEnded)
                return false;

            IHeroModel hero = this.GetModel<IHeroModel>();
            if (hero.Health.Value <= 0)
                return false;

            ITurnSystem turn = this.GetSystem<ITurnSystem>();
            if (!turn.IsPlayerTurn)
                return false;

            if (IsDragging)
                return false;

            if (IsAnimating)
                return false;

            return true;
        }

        public bool CanHover()
        {
            if (IsDragging) return false;
            if (IsAnimating) return false;
            return CanInteract();
        }

        public bool CanEndTurn()
        {
            return CanInteract();
        }
    }
}