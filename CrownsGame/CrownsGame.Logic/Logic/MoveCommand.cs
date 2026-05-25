/*
 * Scopul fisierului: Implementeaza modelul Command pentru actiunea de mutare, permitand functionalitati de Undo si Redo.
 * Autor: 
 */
using CrownsGame.Core;

namespace CrownsGame.Logic
{
    /// <summary>
    /// Reprezinta comanda de schimbare a starii unei celule de pe tabla de joc.
    /// Aceasta clasa stocheaza informatiile necesare atat pentru aplicarea mutarii, cat si pentru revenirea la starea anterioara.
    /// </summary>
    public class MoveCommand : ICommand
    {
        private Board _board;
        private int _row;
        private int _col;
        private CellState _previousState;
        private CellState _newState;

        /// <summary>
        /// Initializeaza o noua instanta a comenzii de mutare, capturand starea actuala a celulei pentru a permite anularea ulterioara.
        /// </summary>
        /// <param name="board">Referinta catre tabla de joc pe care se aplica modificarea.</param>
        /// <param name="row">Randul celulei vizate.</param>
        /// <param name="col">Coloana celulei vizate.</param>
        /// <param name="newState">Noua stare (CellState) ce urmeaza a fi aplicata.</param>
        public MoveCommand(Board board, int row, int col, CellState newState)
        {
            _board = board;
            _row = row;
            _col = col;
            _newState = newState;
            // Salvam starea curenta inainte de modificare pentru a putea face Undo
            _previousState = board.GetCell(row, col).State;
        }

        /// <summary>
        /// Aplica efectiv mutarea pe tabla de joc prin setarea noii stari in celula corespunzatoare.
        /// </summary>
        public void Execute()
        {
            _board.SetCellState(_row, _col, _newState);
        }

        /// <summary>
        /// Revine la starea anterioara a celulei folosind informatiile salvate in momentul instantiarii comenzii.
        /// </summary>
        public void Undo()
        {
            _board.SetCellState(_row, _col, _previousState);
        }
    }
}