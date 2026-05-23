using System.Collections.Generic;
using CrownsGame.Core;
using CrownsGame.Logic;

namespace CrownsGame.AI
{
    public class MiniSolver
    {
        private readonly IGameStrategy _strategy;
        private int _n; // dimensiunea boardului
        private int _k; // coroane per rând/col/regiune

        public MiniSolver(Validator validator, IGameStrategy strategy)
        {
            _strategy = strategy;
        }

        public bool IsSolvable(Board board)
        {
            _n = board.Size;
            _k = _strategy.GetRequiredCrowns();

            int[] rowCount    = new int[_n];
            int[] colCount    = new int[_n];
            int[] regionCount = new int[_n];
            bool[,] placed    = new bool[_n, _n];
            int[,]  adjCount  = new int[_n, _n]; // câte coroane vecine blochează celula

            return Solve(board, 0, rowCount, colCount, regionCount, placed, adjCount);
        }

        private bool Solve(Board board, int row,
            int[] rowCount, int[] colCount, int[] regionCount,
            bool[,] placed, int[,] adjCount)
        {
            if (row == _n)
            {
                for (int c = 0; c < _n; c++)
                    if (colCount[c] != _k) return false;
                for (int reg = 0; reg < _n; reg++)
                    if (regionCount[reg] != _k) return false;
                return true;
            }

            if (rowCount[row] == _k)
                return Solve(board, row + 1, rowCount, colCount, regionCount, placed, adjCount);

            int needed = _k - rowCount[row];
            int free   = 0;
            for (int c = 0; c < _n; c++)
                if (!placed[row, c] && adjCount[row, c] == 0 && colCount[c] < _k)
                    free++;
            if (free < needed) return false;

            return PlaceInRow(board, row, 0, needed,
                              rowCount, colCount, regionCount, placed, adjCount);
        }

        private bool PlaceInRow(Board board, int row, int col, int remaining,
            int[] rowCount, int[] colCount, int[] regionCount,
            bool[,] placed, int[,] adjCount)
        {
            if (remaining == 0)
                return Solve(board, row + 1, rowCount, colCount, regionCount, placed, adjCount);
            if (col >= _n) return false;
            if (_n - col < remaining) return false;

            int reg = board.GetCell(row, col).RegionId;
            bool canPlace = !placed[row, col]
                         && adjCount[row, col] == 0
                         && colCount[col] < _k
                         && regionCount[reg] < _k;

            if (canPlace)
            {
                // Plasează
                placed[row, col] = true;
                rowCount[row]++;
                colCount[col]++;
                regionCount[reg]++;
                AddAdj(adjCount, row, col, +1);

                if (PlaceInRow(board, row, col + 1, remaining - 1,
                               rowCount, colCount, regionCount, placed, adjCount))
                    return true;

                // Backtrack
                placed[row, col] = false;
                rowCount[row]--;
                colCount[col]--;
                regionCount[reg]--;
                AddAdj(adjCount, row, col, -1);
            }

            return PlaceInRow(board, row, col + 1, remaining,
                              rowCount, colCount, regionCount, placed, adjCount);
        }

        private void AddAdj(int[,] adj, int row, int col, int delta)
        {
            for (int dr = -1; dr <= 1; dr++)
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int nr = row + dr, nc = col + dc;
                if (nr >= 0 && nr < _n && nc >= 0 && nc < _n)
                    adj[nr, nc] += delta;
            }
        }
    }
}