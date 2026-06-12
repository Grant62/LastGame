using QFramework;

namespace Features.Combat.Interaction
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