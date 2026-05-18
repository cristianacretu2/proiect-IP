using System.Collections.Generic;

namespace CrownsGame.Core
{
    /// <summary>
    /// Reprezintă tabla de joc fizică, formată dintr-o matrice de celule.
    /// </summary>
    public class Board
    {
        private int _size;
        private int _crownsPerGroup;
        private Cell[,] _grid;

        public int Size { get { return _size; } }

        public Board(int size, int crownsPerGroup)
        {
            _size = size;
            _crownsPerGroup = crownsPerGroup;
            _grid = new Cell[size, size];
        }

        /// <summary>
        /// Setează o celulă la o poziție specifică în faza de inițializare.
        /// </summary>
        public void InitializeCell(int row, int col, int regionId)
        {
            _grid[row, col] = new Cell(regionId);
        }

        public Cell GetCell(int row, int col)
        {
            return _grid[row, col];
        }

        public void SetCellState(int row, int col, CellState newState)
        {
            _grid[row, col].State = newState;
        }
    }
}