using Features.Combat.View.Board;
using QFramework;
using UnityEngine;

namespace Features.Combat.Utility
{
    public interface IBoardAccess : IUtility
    {
        BoardView Board { get; }
        Transform HeroTransform { get; }
    }

    public class BoardAccess : IBoardAccess
    {
        public BoardView Board { get; }
        public Transform HeroTransform { get; }

        public BoardAccess(BoardView board, Transform heroTransform)
        {
            Board = board;
            HeroTransform = heroTransform;
        }
    }
}