using CrownsGame.Core;

namespace CrownsGame.Application
{
    /// <summary>
    /// Reprezintă starea curentă a unei partide, incluzând tabla, scorul și statisticile.
    /// </summary>
    public class GameState
    {
        private Board _board;
        private int _score;
        private int _mistakes;
        private bool _isVictory;

        public Board Board { get { return _board; } }
        public int Score { get { return _score; } set { _score = value; } }
        public int Mistakes { get { return _mistakes; } set { _mistakes = value; } }
        public bool IsVictory { get { return _isVictory; } set { _isVictory = value; } }

        public GameState(Board board)
        {
            _board = board;
            _score = 0;
            _mistakes = 0;
            _isVictory = false;
        }
    }
}