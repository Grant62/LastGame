using Features.Combat.UI.Board;
using QFramework;

namespace Features.Combat.UI
{
    public interface IBoardAccess : IUtility
    {
        BoardPanel Board { get; }
    }

    public class BoardAccess : IBoardAccess
    {
        public BoardPanel Board { get; }

        public BoardAccess(BoardPanel board)
        {
            Board = board;
        }
    }
}