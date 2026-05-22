using QFramework;

namespace Features.Combat.System
{
    public interface ITurnSystem : ISystem
    {
        int TurnCount { get; }
        bool IsPlayerTurn { get; }
        void StartBattle();
        void EndPlayerTurn();
    }
}