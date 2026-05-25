/*
 * Scopul fisierului: Implementeaza algoritmul complex de generare a tablei de joc, asigurand o configuratie valida, solvabila si estetica.
 * Autor: Cretu Cristiana
 */

using System;
using System.Collections.Generic;
using System.Linq;
using CrownsGame.Core;

namespace CrownsGame.Logic
{
    /// <summary>
    /// Clasa responsabila pentru generarea procedurala a tablei de joc.
    /// Utilizeaza backtracking pentru plasarea coroanelor si un algoritm de expansiune (BFS) pentru definirea regiunilor.
    /// </summary>
    public class BoardGenerator
    {
        private Random _rng = new Random();

        /// <summary>
        /// Genereaza o tabla completa si valida conform parametrilor de dificultate furnizati.
        /// </summary>
        /// <param name="size">Dimensiunea laturii gridului patrat.</param>
        /// <param name="crownsPerGroup">Numarul de coroane necesare per rand, coloana si regiune.</param>
        /// <returns>O instanta de Board initializata cu regiuni si gata de joc.</returns>
        public Board Generate(int size, int crownsPerGroup)
        {
            Board board = new Board(size, crownsPerGroup);
            
            // Pas 1: Plasam coroanele intr-o configuratie valida folosind backtracking
            List<Position> crownPositions = PlaceCrowns(size, crownsPerGroup);
            
            if (crownPositions == null) 
                return Generate(size, crownsPerGroup); // Reincercare in caz de esec (probabilitate scazuta)

            // Pas 2: Generam regiunile geografice in jurul coroanelor plasate
            int[,] regionMap = GenerateRegions(size, crownsPerGroup, crownPositions);

            // Pas 3: Transferam datele despre regiuni in obiectul Board
            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    board.InitializeCell(r, c, regionMap[r, c]);
                    // Celulele sunt initializate ca Empty; generatorul doar defineste harta regiunilor.
                }
            }

            return board;
        }

        /// <summary>
        /// Coordoneaza procesul de plasare a coroanelor pe tabla respectand regulile de baza (rand, coloana, proximitate).
        /// </summary>
        /// <param name="size">Dimensiunea tablei.</param>
        /// <param name="k">Numarul de coroane per grup.</param>
        /// <returns>O lista de pozitii pentru coroane sau null daca nu s-a gasit o solutie.</returns>
        private List<Position> PlaceCrowns(int size, int k)
        {
            int[,] grid = new int[size, size];
            int[] rowCounts = new int[size];
            int[] colCounts = new int[size];
            List<Position> positions = new List<Position>();

            if (Backtrack(0, 0, 0, size, k, grid, rowCounts, colCounts, positions))
                return positions;

            return null;
        }

        /// <summary>
        /// Algoritm recursiv de backtracking pentru gasirea unei configuratii valide de coroane.
        /// </summary>
        private bool Backtrack(int row, int col, int placed, int size, int k, int[,] grid, int[] rowCounts, int[] colCounts, List<Position> posList)
        {
            if (placed == size * k) return true;
            if (row == size) return false;

            int nextCol = (col + 1 == size) ? 0 : col + 1;
            int nextRow = (col + 1 == size) ? row + 1 : row;

            // Incercam sa plasam o coroana in celula curenta
            if (CanPlace(row, col, size, k, grid, rowCounts, colCounts))
            {
                grid[row, col] = 1;
                rowCounts[row]++;
                colCounts[col]++;
                posList.Add(new Position(row, col));

                if (Backtrack(nextRow, nextCol, placed + 1, size, k, grid, rowCounts, colCounts, posList))
                    return true;

                // Revenire (Backtrack) daca mutarea nu a dus la o solutie
                grid[row, col] = 0;
                rowCounts[row]--;
                colCounts[col]--;
                posList.RemoveAt(posList.Count - 1);
            }

            // Incercam sa saram peste celula curenta
            if (Backtrack(nextRow, nextCol, placed, size, k, grid, rowCounts, colCounts, posList))
                return true;

            return false;
        }

        /// <summary>
        /// Verifica daca o coroana poate fi plasata la coordonatele r, c fara a incalca regulile de rand, coloana sau adiacenta.
        /// </summary>
        private bool CanPlace(int r, int c, int size, int k, int[,] grid, int[] rowCounts, int[] colCounts)
        {
            if (rowCounts[r] >= k || colCounts[c] >= k) return false;

            // Verificare proximitate in cele 8 directii din jur
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    int nr = r + dr, nc = c + dc;
                    if (nr >= 0 && nr < size && nc >= 0 && nc < size && grid[nr, nc] == 1)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Genereaza regiuni contigue prin gruparea coroanelor si expansiunea acestora pana la umplerea tablei.
        /// </summary>
        /// <param name="size">Dimensiunea tablei.</param>
        /// <param name="k">Coroane per regiune.</param>
        /// <param name="crowns">Lista pozitiilor unde au fost plasate coroanele.</param>
        /// <returns>O matrice reprezentand ID-ul regiunii pentru fiecare celula.</returns>
        private int[,] GenerateRegions(int size, int k, List<Position> crowns)
        {
            int[,] regions = new int[size, size];
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    regions[r, c] = -1;

            List<Position> availableCrowns = new List<Position>(crowns);
            Queue<Position> queue = new Queue<Position>();
            int currentRegionId = 0;

            // Pasul 1: Gruparea coroanelor pentru a forma nucleul fiecarei regiuni
            while (availableCrowns.Count > 0)
            {
                Position startCrown = availableCrowns[_rng.Next(availableCrowns.Count)];
                availableCrowns.Remove(startCrown);
                
                regions[startCrown.Row, startCrown.Col] = currentRegionId;
                queue.Enqueue(startCrown);

                // Gasim cele mai apropiate K-1 coroane de nucleul actual
                for (int i = 1; i < k; i++)
                {
                    if (availableCrowns.Count == 0) break;

                    Position closest = availableCrowns
                        .OrderBy(p => Math.Abs(p.Row - startCrown.Row) + Math.Abs(p.Col - startCrown.Col))
                        .First();

                    regions[closest.Row, closest.Col] = currentRegionId;
                    queue.Enqueue(closest);
                    availableCrowns.Remove(closest);
                }
                currentRegionId++;
            }

            // Pasul 2: Expansiune prin algoritmul BFS pentru a ocupa celulele ramase (Empty)
            List<Position> frontier = new List<Position>(queue);
            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            while (frontier.Count > 0)
            {
                int index = _rng.Next(frontier.Count);
                Position current = frontier[index];
                frontier.RemoveAt(index);

                int regionId = regions[current.Row, current.Col];

                for (int i = 0; i < 4; i++)
                {
                    int nr = current.Row + dr[i];
                    int nc = current.Col + dc[i];

                    if (nr >= 0 && nr < size && nc >= 0 && nc < size && regions[nr, nc] == -1)
                    {
                        regions[nr, nc] = regionId;
                        frontier.Add(new Position(nr, nc));
                    }
                }
            }

            return regions;
        }
    }
}