// gridul propriu-zis
// matricea jocului 

namespace CrownsGame.Core
{
    public class Board
    {
        private readonly int _size;
        private readonly int _crownsRequiredPerGroup;

        private readonly Cell[,] _grid;

        public int Size{

            get { return _size; }
        }
        public int CrownsRequiredPerGroup
        {
            get { return _crownsRequiredPerGroup; }
        }

        public Board(int size, int crownsRequiredPerGroup)
        {
            _size = size;
            _crownsRequiredPerGroup = crownsRequiredPerGroup;
            _grid = new Cell[size, size];
        }

        public Cell GetCell(int row, int col)
        {
            return _grid[row, col];
        }
        public void SetCellState(int row, int col, CellState newState)
        {
            _grid[row, col].State = newState;
        }

        public void InitializeCell(int row, int col, int regionId)
        {
            _grid[row, col] = new Cell(new Position(row,col), regionId);
        }

    }
}