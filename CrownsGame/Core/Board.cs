namespace CrownsGame.Core
{
    public class Board
    {
        private int _size;
        private int _crownsPerGroup;
        private Cell[,] _grid;

        public int Size => _size;
        public int CrownsPerGroup => _crownsPerGroup;

        public Board(int size, int crownsPerGroup)
        {
            _size = size;
            _crownsPerGroup = crownsPerGroup;
            _grid = new Cell[size, size];
        }

        public void InitializeCell(int row, int col, int regionId)
        {
            _grid[row, col] = new Cell(regionId);
        }

        public Cell GetCell(int row, int col) => _grid[row, col];

        public void SetCellState(int row, int col, CellState newState) => _grid[row, col].State = newState;

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