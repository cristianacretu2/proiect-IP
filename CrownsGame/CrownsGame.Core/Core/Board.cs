/*
 * Scopul fisierului: Defineste clasa Board care gestioneaza matricea de joc si starea globala a tablei.
 * Autor: Sebastian Mihai Lungu
 */

namespace CrownsGame.Core
{
    /// <summary>
    /// Reprezinta tabla de joc formata dintr-o matrice de celule, gestionand dimensiunea si regulile de plasare.
    /// </summary>
    public class Board
    {
        private int _size;
        private int _crownsPerGroup;
        private Cell[,] _grid;

        /// <summary>
        /// Dimensiunea laturii tablei patratice.
        /// </summary>
        public int Size => _size;

        /// <summary>
        /// Numarul de coroane necesar pentru fiecare rand, coloana sau regiune pentru a castiga.
        /// </summary>
        public int CrownsPerGroup => _crownsPerGroup;

        /// <summary>
        /// Initializeaza o noua instanta a tablei de joc cu dimensiunile specificate.
        /// </summary>
        /// <param name="size">Numarul de randuri si coloane.</param>
        /// <param name="crownsPerGroup">Tinta de coroane per grup logic.</param>
        public Board(int size, int crownsPerGroup)
        {
            _size = size;
            _crownsPerGroup = crownsPerGroup;
            _grid = new Cell[size, size];
        }

        /// <summary>
        /// Aloca o celula noua la coordonatele specificate si ii atribuie o regiune colorata.
        /// </summary>
        /// <param name="row">Indexul randului.</param>
        /// <param name="col">Indexul coloanei.</param>
        /// <param name="regionId">ID-ul regiunii din care face parte celula.</param>
        public void InitializeCell(int row, int col, int regionId)
        {
            _grid[row, col] = new Cell(regionId);
        }

        /// <summary>
        /// Returneaza obiectul Cell de la coordonatele solicitate pentru inspectie sau modificare.
        /// </summary>
        /// <param name="row">Randul celulei.</param>
        /// <param name="col">Coloana celulei.</param>
        /// <returns>Obiectul de tip Cell aflat la pozitia respectiva.</returns>
        public Cell GetCell(int row, int col) => _grid[row, col];

        /// <summary>
        /// Actualizeaza starea vizuala sau logica a unei celule specifice.
        /// </summary>
        /// <param name="row">Randul vizat.</param>
        /// <param name="col">Coloana vizata.</param>
        /// <param name="newState">Noua stare (Empty, Marked sau Crown).</param>
        public void SetCellState(int row, int col, CellState newState) => _grid[row, col].State = newState;

        /// <summary>
        /// Creeaza o copie profunda a tablei actuale, utila pentru mecanisme de tip Undo sau simulari.
        /// </summary>
        /// <returns>O noua instanta Board cu aceleasi date ca cea curenta.</returns>
        public Board Clone()
        {
            var newBoard = new Board(_size, _crownsPerGroup);
            for (int r = 0; r < _size; r++)
                for (int c = 0; c < _size; c++)
                {
                    newBoard.InitializeCell(r, c, _grid[r, c].RegionId);
                    newBoard.SetCellState(r, c, _grid[r, c].State);
                }
            return newBoard;
        }
    }
}