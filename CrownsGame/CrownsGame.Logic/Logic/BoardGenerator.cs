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
            int k    = strategy.GetRequiredCrowns();

            for (int attempt = 0; attempt < 10_000; attempt++)
            {
                // Pas 1: plasează coroanele rând cu rând
                var crowns = TryPlaceOnce(size, k, maxCalls: 50_000);
                if (crowns == null) continue;

                // Pas 2: expandează regiunile prin BFS
                int[,] grid = ExpandRegions(size, crowns, k);

                // Pas 3: verifică că fiecare regiune poate primi k coroane
                if (!AllRegionsValid(grid, size, k, crowns)) continue;

                // Pas 4: construiește Board-ul
                return BuildBoard(size, k, grid, crowns);
            }

            throw new InvalidOperationException(
                $"Nu s-a putut genera un board valid (size={size}, k={k})");
        }

        // ─── Pas 1: backtracking rând cu rând ────────────────────────────────

        private List<(int r, int c)>? TryPlaceOnce(int size, int k, int maxCalls)
        {
            var colCount = new int[size];
            var adj      = new int[size, size];
            var result   = new List<(int r, int c)>(size * k);
            int calls    = 0;

            var colOrders = new int[size][];
            for (int r = 0; r < size; r++)
            {
                colOrders[r] = Enumerable.Range(0, size).ToArray();
                Shuffle(colOrders[r]);
            }

            bool? Bt(int row, int colIdx, int chosen)
            {
                if (++calls > maxCalls) return null;

                if (chosen == k)
                {
                    if (row + 1 == size)
                        return colCount.All(x => x == k);

                    int rowsLeft = size - row - 1;
                    for (int c = 0; c < size; c++)
                        if (colCount[c] + rowsLeft < k) return false;

                    return Bt(row + 1, 0, 0);
                }

                if (size - colIdx < k - chosen) return false;

                var order = colOrders[row];
                for (int i = colIdx; i < size; i++)
                {
                    int c = order[i];
                    if (colCount[c] >= k) continue;
                    if (adj[row, c] > 0)  continue;

                    result.Add((row, c));
                    colCount[c]++;
                    AddAdj(adj, row, c, size, +1);

                    var res = Bt(row, i + 1, chosen + 1);
                    if (res == true)  return true;
                    if (res == null)  return null;

                    result.RemoveAt(result.Count - 1);
                    colCount[c]--;
                    AddAdj(adj, row, c, size, -1);
                }

                return false;
            }

            return Bt(0, 0, 0) == true ? result : null;
        }

        // ─── Pas 2: BFS expansion ─────────────────────────────────────────────

        private int[,] ExpandRegions(int size, List<(int r, int c)> crowns, int k)
        {
            var grid  = new int[size, size];
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    grid[r, c] = -1;

            var queue = new Queue<(int r, int c)>();
            for (int i = 0; i < crowns.Count; i++)
            {
                var (r, c) = crowns[i];
                grid[r, c] = i / k; // regionId
                queue.Enqueue((r, c));
            }

            int[] dr = { -1, 1,  0, 0 };
            int[] dc = {  0, 0, -1, 1 };

            while (queue.Count > 0)
            {
                var (cr, cc) = queue.Dequeue();
                int region   = grid[cr, cc];

                var dirs = Enumerable.Range(0, 4).OrderBy(_ => _random.Next()).ToArray();
                foreach (int i in dirs)
                {
                    int nr = cr + dr[i], nc = cc + dc[i];
                    if (nr < 0 || nr >= size || nc < 0 || nc >= size) continue;
                    if (grid[nr, nc] != -1) continue;

                    grid[nr, nc] = region;
                    queue.Enqueue((nr, nc));
                }
            }

            return grid;
        }

        // ─── Pas 3: validare regiuni ──────────────────────────────────────────

        private bool AllRegionsValid(int[,] grid, int size, int k,
                                     List<(int r, int c)> crowns)
        {
            for (int regionId = 0; regionId < size; regionId++)
                if (!RegionCanFitKCrowns(grid, size, regionId, k, crowns))
                    return false;
            return true;
        }

        private bool RegionCanFitKCrowns(int[,] grid, int size, int regionId, int k,
                                          List<(int r, int c)> crowns)
        {
            // Celulele care aparțin regiunii
            var cells = new List<(int r, int c)>();
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    if (grid[r, c] == regionId)
                        cells.Add((r, c));

            // Blocaj inițial din coroanele altor regiuni
            var initAdj = new int[size, size];
            for (int i = 0; i < crowns.Count; i++)
            {
                if (i / k == regionId) continue;
                var (cr, cc) = crowns[i];
                for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = cr + dr, nc = cc + dc;
                    if (nr >= 0 && nr < size && nc >= 0 && nc < size)
                        initAdj[nr, nc]++;
                }
            }

            // Copiem pentru backtracking
            var adj = (int[,])initAdj.Clone();

            bool Bt(int idx, int remaining)
            {
                if (remaining == 0) return true;
                if (cells.Count - idx < remaining) return false;

                var (r, c) = cells[idx];

                // Cu coroană în (r,c)
                if (adj[r, c] == 0)
                {
                    for (int dr = -1; dr <= 1; dr++)
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        if (dr == 0 && dc == 0) continue;
                        int nr = r + dr, nc = c + dc;
                        if (nr >= 0 && nr < size && nc >= 0 && nc < size)
                            adj[nr, nc]++;
                    }

                    if (Bt(idx + 1, remaining - 1)) return true;

                    for (int dr = -1; dr <= 1; dr++)
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        if (dr == 0 && dc == 0) continue;
                        int nr = r + dr, nc = c + dc;
                        if (nr >= 0 && nr < size && nc >= 0 && nc < size)
                            adj[nr, nc]--;
                    }
                }

                // Fără coroană în (r,c)
                return Bt(idx + 1, remaining);
            }

            return Bt(0, k);
        }

        // ─── Pas 4: construiește Board ─────────────────────────────────────────

        private Board BuildBoard(int size, int k, int[,] grid,
                                  List<(int r, int c)> crowns)
        {
            var board = new Board(size, k);

            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    board.InitializeCell(r, c, grid[r, c]);

            // Marcăm celulele cu coroane
            foreach (var (r, c) in crowns)
                board.GetCell(r, c).InitializeRegion(grid[r, c]);

            return board;
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

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

        private void Shuffle(int[] arr)
        {
            for (int i = arr.Length - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }
    }
}