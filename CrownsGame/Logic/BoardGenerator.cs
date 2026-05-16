using System;
using System.Collections.Generic;
using CrownsGame.Core;

namespace CrownsGame.Logic
{
    /// <summary>
    /// Algoritm pentru generarea procedurală a regiunilor și a soluției.
    /// </summary>
    public class BoardGenerator
    {
        private Random _random;

        public BoardGenerator()
        {
            _random = new Random();
        }

        public Board Generate(IGameStrategy strategy)
        {
            int size = strategy.GetBoardSize();
            int crowns = strategy.GetRequiredCrowns();
            Board board = new Board(size, crowns);

            // Pas 1: Umplem tabla cu celule "goale" (-1)
            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    board.InitializeCell(r, c, -1);
                }
            }

            // Pas 2: Alegem "semințele" (locațiile viitoarelor coroane)
            // Pentru simplitate, generăm o permutare de coloane pentru rânduri
            List<int> cols = new List<int>();
            for (int i = 0; i < size; i++) cols.Add(i);

            for (int r = 0; r < size; r++)
            {
                int idx = _random.Next(cols.Count);
                int selectedCol = cols[idx];
                
                // Setăm ID-ul regiunii la rândul respectiv pentru a marca nucleul
                board.GetCell(r, selectedCol).InitializeRegion(r);
                cols.RemoveAt(idx);
            }

            // Pas 3: Flood Fill (expandăm regiunile)
            ExpandRegions(board);

            return board;
        }

        private void ExpandRegions(Board board)
        {
            bool hasUnassigned = true;
            while (hasUnassigned)
            {
                hasUnassigned = false;
                for (int r = 0; r < board.Size; r++)
                {
                    for (int c = 0; c < board.Size; c++)
                    {
                        if (board.GetCell(r, c).RegionId == -1)
                        {
                            hasUnassigned = true;
                            AssignNeighborRegion(board, r, c);
                        }
                    }
                }
            }
        }

        private void AssignNeighborRegion(Board board, int r, int c)
        {
            // Verificăm vecinii (Sus, Jos, Stânga, Dreapta)
            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nr = r + dr[i];
                int nc = c + dc[i];
                if (nr >= 0 && nr < board.Size && nc >= 0 && nc < board.Size)
                {
                    int reg = board.GetCell(nr, nc).RegionId;
                    if (reg != -1)
                    {
                        board.GetCell(r, c).InitializeRegion(reg);
                        return;
                    }
                }
            }
        }
    }
}