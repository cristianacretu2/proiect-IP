/*
 * Scopul fisierului: Implementeaza un algoritm de cautare pentru a determina daca o configuratie a tablei are solutie.
 * Autor: Radani Antonia
 */

using System.Collections.Generic;
using CrownsGame.Core;
using CrownsGame.Logic;

namespace CrownsGame.AI
{
    /// <summary>
    /// Componenta de calcul care utilizeaza backtracking pentru a verifica potentialul de finalizare a unui joc.
    /// </summary>
    public class MiniSolver
    {
        private readonly IGameStrategy _strategy;
        private int _n; // dimensiunea boardului
        private int _k; // coroane per rând/col/regiune

        /// <summary>
        /// Initializeaza solver-ul cu strategia de joc activa.
        /// </summary>
        /// <param name="validator">Validatorul pentru regulile de joc.</param>
        /// <param name="strategy">Strategia care defineste conditiile de victorie.</param>
        public MiniSolver(Validator validator, IGameStrategy strategy)
        {
            _strategy = strategy;
        }

        /// <summary>
        /// Determina daca starea curenta a tablei mai permite plasarea coroanelor ramase conform regulilor.
        /// </summary>
        /// <param name="board">Tabla de joc de analizat.</param>
        /// <returns>True daca exista cel putin o solutie valida pornind de la starea actuala.</returns>
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


        /// <summary>
        /// Metoda recursiva care exploreaza posibilitatile de plasare a coroanelor pe fiecare rand.
        /// </summary>
        /// <returns>Statusul explorarii caii curente.</returns>
        private bool Solve(Board board, int row,
            int[] rowCount, int[] colCount, int[] regionCount,
            bool[,] placed, int[,] adjCount)
        {
            // Verificarea conditiilor de oprire si validarea coloanelor/regiunilor la final.
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

        /// <summary>
        /// Metoda recursiva de tip helper care incearca plasarea numarului necesar de coroane pe un singur rand, 
        /// respectand restrictiile de coloana, regiune si adiacenta.
        /// </summary>
        /// <param name="board">Instanta tablei de joc pentru accesarea datelor despre regiuni.</param>
        /// <param name="row">Indexul randului curent in care se fac plasarile.</param>
        /// <param name="col">Indexul coloanei curente care este evaluata.</param>
        /// <param name="remaining">Numarul de coroane care mai trebuie plasate pe randul curent pentru a indeplini conditia de victorie.</param>
        /// <param name="rowCount">Vector pentru monitorizarea numarului de coroane pe fiecare rand.</param>
        /// <param name="colCount">Vector pentru monitorizarea numarului de coroane pe fiecare coloana.</param>
        /// <param name="regionCount">Vector pentru monitorizarea distributiei coroanelor pe regiuni colorate.</param>
        /// <param name="placed">Matrice booleana care retine locatiile unde au fost deja plasate coroane.</param>
        /// <param name="adjCount">Matrice de intregi care urmareste celulele blocate de coroanele vecine (regula de non-proximitate).</param>
        /// <returns>True daca s-a gasit o configuratie valida pentru restul tablei pornind de la aceasta plasare, false in caz contrar.</returns>
        private bool PlaceInRow(Board board, int row, int col, int remaining,
            int[] rowCount, int[] colCount, int[] regionCount,
            bool[,] placed, int[,] adjCount)
        {
            // Daca pe randul curent s-au plasat toate coroanele necesare, trecem la urmatorul rand.
            if (remaining == 0)
                return Solve(board, row + 1, rowCount, colCount, regionCount, placed, adjCount);
            // Verificari de siguranta pentru limitele tablei si posibilitatea matematica de a mai plasa coroane.
            if (col >= _n) return false;
            if (_n - col < remaining) return false;

            int reg = board.GetCell(row, col).RegionId;
            // Verificarea tuturor constrangerilor logice inainte de a plasa o coroana in celula curenta.
            bool canPlace = !placed[row, col]
                         && adjCount[row, col] == 0
                         && colCount[col] < _k
                         && regionCount[reg] < _k;

            if (canPlace)
            {
                // Se marcheaza plasarea si se actualizeaza starea structurilor de monitorizare (Pasul de inaintare in Backtracking).
                placed[row, col] = true;
                rowCount[row]++;
                colCount[col]++;
                regionCount[reg]++;
                AddAdj(adjCount, row, col, +1);

                // Se exploreaza recursiv posibilitatea plasarii urmatoarei coroane pe aceeasi linie.
                if (PlaceInRow(board, row, col + 1, remaining - 1,
                               rowCount, colCount, regionCount, placed, adjCount))
                    return true;

                // Daca mutarea nu a dus la o solutie, se anuleaza modificarile (Pasul de revenire in Backtracking).
                placed[row, col] = false;
                rowCount[row]--;
                colCount[col]--;
                regionCount[reg]--;
                AddAdj(adjCount, row, col, -1);
            }
            // Se incearca si varianta in care celula curenta ramane libera, trecand la urmatoarea coloana.
            return PlaceInRow(board, row, col + 1, remaining,
                              rowCount, colCount, regionCount, placed, adjCount);
        }

        /// <summary>
        /// Gestioneaza marcarea celulelor adiacente pentru a respecta regula de non-proximitate a coroanelor.
        /// </summary>
        /// <param name="adj">Matricea de proximitate.</param>
        /// <param name="row">Randul sursa.</param>
        /// <param name="col">Coloana sursa.</param>
        /// <param name="delta">Valoarea de adunare/scadere pentru marcare (1 sau -1).</param>
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