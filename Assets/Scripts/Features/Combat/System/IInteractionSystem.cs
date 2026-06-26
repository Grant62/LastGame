using QFramework;

namespace Features.Combat.System
{
    public interface IInteractionSystem : ISystem
    {
        bool IsDragging { get; }
        void BeginDrag();
        void EndDrag();
        void BeginAnimation();
        void EndAnimation();
        bool CanInteract();
        bool CanHover();
        bool CanEndTurn();
    }
}