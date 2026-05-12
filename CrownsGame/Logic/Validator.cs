using CrownsGame.Core;
using System.Collections.Generic;

namespace CrownsGame.Logic
{
    /// <summary>
    /// Clasa responsabila de validarea mutarilor individuale conform regulamentului.
    /// </summary>
    public class Validator
    {
        private IGameStrategy _strategy;

        public Validator(IGameStrategy strategy)
        {
            _strategy = strategy;
        }
        private bool CheckAdjacency(Board board, int row, int col)
        {
            for (int r = row - 1; r <= row + 1; r++)
            {
                for (int c = col - 1; c <= col + 1; c++)
                {
                    if (r >= 0 && r < board.Size && c >= 0 && c < board.Size && !(r == row && c == col))
                    {
                        if (board.GetCell(r, c).State == CellState.Crown)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool CheckRowLimit(Board board, int row)
        {
            int count = 0;
            for (int c = 0; c < board.Size; c++)
            {
                if (board.GetCell(row, c).State == CellState.Crown)
                {
                    count++;
                }
            }
            return count < _strategy.GetRequiredCrowns();
        }

        private bool CheckColumnLimit(Board board, int col)
        {
            int count = 0;
            for (int r = 0; r < board.Size; r++)
            {
                if (board.GetCell(r, col).State == CellState.Crown)
                {
                    count++;
                }
            }
            return count < _strategy.GetRequiredCrowns();
        }

        private bool CheckRegionLimit(Board board, int regionId)
        {
            int count = 0;
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    Cell cell = board.GetCell(r, c);
                    if (cell.RegionId == regionId && cell.State == CellState.Crown)
                    {
                        count++;
                    }
                }
            }
            return count < _strategy.GetRequiredCrowns();
        }
        /// <summary>
        /// Verifica daca o coroana poate fi plasata la coordonatele specificate.
        /// </summary>
        /// <param name="board">Tabla de joc curenta.</param>
        /// <param name="row">Randul tinta.</param>
        /// <param name="col">Coloana tinta.</param>
        /// <returns>True daca toate regulile de plasare sunt respectate.</returns>
        public bool IsMoveValid(Board board, int row, int col)
        {
            // 1. verificam regula de adiacenta (sa nu fie in jurul ei alte coroane, cei 8 vecini)
            if (CheckAdjacency(board, row, col) == false)
            {
                return false;
            }

            // 2. verificam sa nu depasim nr N de coroane pe rand
            if (CheckRowLimit(board, row) == false)
            {
                return false;
            }

            // 3. verificam sa nu depasim nr N de coroane pe coloana
            if (CheckColumnLimit(board, col) == false)
            {
                return false;
            }

            // 4. verificam sa nu depasim numarul N de coroane in regiune 
            int regionId = board.GetCell(row, col).RegionId;
            if (CheckRegionLimit(board, regionId) == false)
            {
                return false;
            }

            return true;
        }
    }
}