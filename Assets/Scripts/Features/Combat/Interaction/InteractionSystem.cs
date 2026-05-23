using Features.Combat.System;
using Features.Hero.Model;
using QFramework;

namespace Features.Combat.Interaction
{
    public class InteractionSystem : AbstractSystem, IInteractionSystem
    {
        public bool IsDragging { get; private set; }
        public bool IsArrowing { get; private set; }

        protected override void OnInit()
        {
            this.RegisterEvent<DragStartEvent>(_ => IsDragging = true);
            this.RegisterEvent<DragEndEvent>(_ => IsDragging = false);
            this.RegisterEvent<ArrowStartEvent>(_ => IsArrowing = true);
            this.RegisterEvent<ArrowEndEvent>(_ => IsArrowing = false);
        }

        public bool CanInteract()
        {
            IHeroModel hero = this.GetModel<IHeroModel>();
            if (hero.Health.Value <= 0)
                return false;

            ITurnSystem turn = this.GetSystem<ITurnSystem>();
            if (!turn.IsPlayerTurn)
                return false;

            if (IsDragging)
                return false;

            if (IsArrowing)
                return false;

            // 后续扩展:
            // if (CardSystem.IsDrawingCards) return false;
            // if (UIPanel.IsAnyOpen()) return false;

            return true;
        }

        public bool CanHover()
        {
            if (IsDragging)
                return false;

            // 排除英雄死亡、敌人回合
            return CanInteract();
        }

        public bool CanEndTurn(bool isProcessing = false)
        {
            if (isProcessing)
                return false;

            return CanInteract();
        }
    }
}