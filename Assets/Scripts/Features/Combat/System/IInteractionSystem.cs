using QFramework;

namespace Features.Combat.System
{
    public interface IInteractionSystem : ISystem
    {
        bool IsDragging { get; }
        bool IsAnimating { get; }
        void BeginDrag();
        void EndDrag();
        void BeginAnimation();
        void EndAnimation();
        bool CanInteract();
        bool CanHover();
        bool CanEndTurn();
    }
}