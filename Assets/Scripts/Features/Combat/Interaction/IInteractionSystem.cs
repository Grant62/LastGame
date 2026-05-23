using QFramework;

namespace Features.Combat.Interaction
{
    public interface IInteractionSystem : ISystem
    {
        bool IsDragging { get; }
        bool IsArrowing { get; }
        bool CanInteract();
        bool CanHover();
        bool CanEndTurn(bool isProcessing = false);
    }
}