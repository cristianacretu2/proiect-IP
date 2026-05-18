using CrownsGame.Core;

namespace CrownsGame.Logic
{
    /// <summary>
    /// Reprezintă comanda de schimbare a stării unei celule de pe tabla de joc.
    /// </summary>
    public class MoveCommand : ICommand
    {
        private Board _board;
        private int _row;
        private int _col;
        private CellState _previousState;
        private CellState _newState;

        /// <summary>
        /// Inițializează o nouă instanță a comenzii de mutare.
        /// </summary>
        public MoveCommand(Board board, int row, int col, CellState newState)
        {
            _board = board;
            _row = row;
            _col = col;
            _newState = newState;
            // Salvăm starea curentă înainte de modificare pentru a putea face Undo
            _previousState = board.GetCell(row, col).State;
        }

        public void Execute()
        {
            _board.SetCellState(_row, _col, _newState);
        }

        public void Undo()
        {
            _board.SetCellState(_row, _col, _previousState);
        }
    }
}