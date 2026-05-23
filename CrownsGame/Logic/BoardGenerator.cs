using System;
using System.Collections.Generic;
using System.Linq;
using CrownsGame.Core;

namespace CrownsGame.Logic
{
    public class BoardGenerator
    {
        private readonly Random _random;

        public BoardGenerator()
        {
            _random = new Random();
        }

        public Board Generate(IGameStrategy strategy)
        {
            int size = strategy.GetBoardSize();
            int crownsPerTarget = strategy.GetRequiredCrowns();

            List<(int r, int c)> crownPositions = PlaceAllCrownsWithBacktracking(size, crownsPerTarget);

            Board board = new Board(size, crownsPerTarget);

            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    board.InitializeCell(r, c, -1);

            for (int regionId = 0; regionId < size; regionId++)
                for (int j = 0; j < crownsPerTarget; j++)
                {
                    var pos = crownPositions[regionId * crownsPerTarget + j];
                    board.GetCell(pos.r, pos.c).InitializeRegion(regionId);
                }

            ExpandRegions(board);
            return board;
        }

        private List<(int r, int c)> PlaceAllCrownsWithBacktracking(int size, int k)
        {
            // Amestecăm o dată pentru varietate
            var allCells = Enumerable.Range(0, size * size)
                .Select(i => (r: i / size, c: i % size))
                .OrderBy(_ => _random.Next())
                .ToArray();

            var placed = new List<(int r, int c)>(size * k);
            var usedCells = new HashSet<(int, int)>(); // celule deja ocupate

            int[] rowCount    = new int[size];
            int[] colCount    = new int[size];
            int[,] adjCount   = new int[size, size]; // câte coroane vecine are fiecare celulă

            bool success = Backtrack(0, k, size, allCells,
                                     placed, usedCells,
                                     rowCount, colCount, adjCount);

            if (!success)
                throw new InvalidOperationException(
                    $"Nu s-au putut plasa coroanele pentru size={size}, k={k}");

            return placed;
        }

        private bool Backtrack(
            int crownIndex, int k, int size,
            (int r, int c)[] candidates,
            List<(int r, int c)> placed,
            HashSet<(int, int)> usedCells,
            int[] rowCount, int[] colCount,
            int[,] adjCount)
        {
            int totalCrowns = size * k;
            if (crownIndex == totalCrowns) return true;

            int regionId = crownIndex / k;

            // Câte coroane are deja această regiune
            int crownsInRegion = crownIndex % k; // = câte am pus deja în regionId

            foreach (var cell in candidates)
            {
                if (usedCells.Contains(cell)) continue;

                if (!CanPlace(cell.r, cell.c, size, k,
                              rowCount, colCount, adjCount,
                              placed, regionId, crownsInRegion))
                    continue;

                // Plasează
                placed.Add(cell);
                usedCells.Add(cell);
                rowCount[cell.r]++;
                colCount[cell.c]++;
                AddAdj(adjCount, cell.r, cell.c, size, +1);

                if (Backtrack(crownIndex + 1, k, size, candidates,
                              placed, usedCells,
                              rowCount, colCount, adjCount))
                    return true;

                // Backtrack
                placed.RemoveAt(placed.Count - 1);
                usedCells.Remove(cell);
                rowCount[cell.r]--;
                colCount[cell.c]--;
                AddAdj(adjCount, cell.r, cell.c, size, -1);
            }

            return false;
        }

        private bool CanPlace(
            int r, int c, int size, int k,
            int[] rowCount, int[] colCount, int[,] adjCount,
            List<(int r, int c)> placed, int regionId, int crownsInRegion)
        {
            // 1. Limita globală pe rând/coloană
            if (rowCount[r] >= k) return false;
            if (colCount[c] >= k) return false;

            // 2. Adiacență (inclusiv diagonale) — adjCount > 0 înseamnă că e blocat
            if (adjCount[r, c] > 0) return false;

            // 3. Constrângeri per-regiune:
            //    - aceeași regiune nu poate pune 2 coroane pe același rând sau coloană
            //    - verificăm față de coroanele deja plasate în această regiune
            int startIdx = regionId * k; // indexul primei coroane din regiune în `placed`
            for (int i = startIdx; i < startIdx + crownsInRegion; i++)
            {
                var prev = placed[i];
                if (prev.r == r) return false; // aceeași linie în aceeași regiune
                if (prev.c == c) return false; // aceeași coloană în aceeași regiune
            }

            return true;
        }

        private void AddAdj(int[,] adj, int r, int c, int size, int delta)
        {
            for (int dr = -1; dr <= 1; dr++)
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int nr = r + dr, nc = c + dc;
                if (nr >= 0 && nr < size && nc >= 0 && nc < size)
                    adj[nr, nc] += delta;
            }
        }

        private void ExpandRegions(Board board)
        {
            var queue = new Queue<(int r, int c)>();

            for (int r = 0; r < board.Size; r++)
                for (int c = 0; c < board.Size; c++)
                    if (board.GetCell(r, c).RegionId != -1)
                        queue.Enqueue((r, c));

            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            while (queue.Count > 0)
            {
                var (cr, cc) = queue.Dequeue();
                int region = board.GetCell(cr, cc).RegionId;

                var dirs = Enumerable.Range(0, 4).OrderBy(_ => _random.Next()).ToArray();

                foreach (int i in dirs)
                {
                    int nr = cr + dr[i], nc = cc + dc[i];
                    if (nr < 0 || nr >= board.Size || nc < 0 || nc >= board.Size) continue;
                    if (board.GetCell(nr, nc).RegionId != -1) continue;

                    board.GetCell(nr, nc).InitializeRegion(region);
                    queue.Enqueue((nr, nc));
                }
            }
        }
    }
}