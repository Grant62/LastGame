using QFramework;

namespace Features.Combat.System
{
    public interface IInteractionSystem : ISystem
    {
        bool IsDragging { get; }
        bool IsAnimating { get; set; }
        void BeginDrag();
        void EndDrag();
        bool CanInteract();
        bool CanHover();
        bool CanEndTurn();
    }
}