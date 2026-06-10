using Features.Combat.System;
using Features.Hero.Model;
using QFramework;

namespace Features.Combat.Interaction
{
    public class InteractionSystem : AbstractSystem, IInteractionSystem
    {
        public bool IsDragging { get; private set; }

        protected override void OnInit() { }

        public void BeginDrag()
        {
            IsDragging = true;
        }

        public void EndDrag()
        {
            IsDragging = false;
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

            return true;
        }

        public bool CanHover()
        {
            if (IsDragging)
                return false;

            return CanInteract();
        }

        public bool CanEndTurn()
        {
            return CanInteract();
        }
    }
}