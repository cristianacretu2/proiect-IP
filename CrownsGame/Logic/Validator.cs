using CrownsGame.Core;
using System.Collections.Generic;

namespace CrownsGame.Logic
{
    public class Validator
    {
        private readonly int _requiredCrowns;

        // Putem inițializa validatorul direct cu numărul de coroane
        public Validator(int requiredCrowns)
        {
            _requiredCrowns = requiredCrowns;
        }

        // Sau prin interfața ta originală
        public Validator(IGameStrategy strategy)
        {
            _requiredCrowns = strategy.GetRequiredCrowns();
        }

        public bool IsMoveValid(Board board, int row, int col)
        {
            // Verificăm dacă celula este deja ocupată
            if (board.GetCell(row, col).State == CellState.Crown) return false;

            if (!CheckAdjacency(board, row, col)) return false;
            if (!CheckRowLimit(board, row)) return false;
            if (!CheckColumnLimit(board, col)) return false;

            int regionId = board.GetCell(row, col).RegionId;
            if (!CheckRegionLimit(board, regionId)) return false;

            return true;
        }

        private bool CheckAdjacency(Board board, int row, int col)
        {
            for (int r = row - 1; r <= row + 1; r++)
            {
                for (int c = col - 1; c <= col + 1; c++)
                {
                    if (r >= 0 && r < board.Size && c >= 0 && c < board.Size && !(r == row && c == col))
                    {
                        if (board.GetCell(r, c).State == CellState.Crown) return false;
                    }
                }
            }
            return true;
        }

        private bool CheckRowLimit(Board board, int row)
        {
            int count = 0;
            for (int c = 0; c < board.Size; c++)
                if (board.GetCell(row, c).State == CellState.Crown) count++;
            
            return count < _requiredCrowns;
        }

        private bool CheckColumnLimit(Board board, int col)
        {
            int count = 0;
            for (int r = 0; r < board.Size; r++)
                if (board.GetCell(r, col).State == CellState.Crown) count++;
            
            return count < _requiredCrowns;
        }

        private bool CheckRegionLimit(Board board, int regionId)
        {
            int count = 0;
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    var cell = board.GetCell(r, c);
                    if (cell.RegionId == regionId && cell.State == CellState.Crown)
                        count++;
                }
            }
            return count < _requiredCrowns;
        }
    }
}