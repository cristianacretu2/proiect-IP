using CrownsGame.Core;
using CrownsGame.Logic;

namespace CrownsGame.AI
{
    public class HintEngine
    {
        private readonly Validator _validator;
        private readonly MiniSolver _solver;
        private readonly IGameStrategy _strategy;

        public HintEngine(Validator validator, IGameStrategy strategy)
        {
            _validator = validator;
            _strategy = strategy;
            _solver = new MiniSolver(validator, strategy);
        }

        public Position? GetBestHint(Board board)
        {
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    if (board.GetCell(r, c).State == CellState.Empty)
                    {
                        // Dacă mutarea e validă acum, verificăm dacă e validă și pe termen lung
                        if (_validator.IsMoveValid(board, r, c))
                        {
                            Board temp = board.Clone();
                            temp.SetCellState(r, c, CellState.Crown);
                            
                            if (_solver.IsSolvable(temp))
                            {
                                return new Position(r, c);
                            }
                        }
                    }
                }
            }
            return null; // Nu s-a găsit nicio mutare care să ducă la soluție
        }
    }
}