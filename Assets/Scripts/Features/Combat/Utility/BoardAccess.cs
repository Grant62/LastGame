using Features.Combat.View.Board;
using QFramework;

namespace Features.Combat.Utility
{
    public interface IBoardAccess : IUtility
    {
        BoardView Board { get; }
    }

    public class BoardAccess : IBoardAccess
    {
        public BoardView Board { get; }

        public BoardAccess(BoardView board)
        {
            Board = board;
        }
    }
}