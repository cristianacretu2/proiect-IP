using CrownsGame.Core;
using CrownsGame.Logic;

namespace CrownsGame.Application
{
    /// <summary>
    /// Motorul principal care coordonează fluxul jocului.
    /// </summary>
    public class GameEngine
    {
        private GameState _state;
        private Validator _validator;
        private CommandManager _commandManager;
        private InputHandler _inputHandler;
        private IGameStrategy _strategy;

        public GameState State { get { return _state; } }

        public GameEngine(IGameStrategy strategy)
        {
            _strategy = strategy;
            
            // Folosim generatorul tău existent
            BoardGenerator generator = new BoardGenerator();
            Board newBoard = generator.Generate(strategy);

            _state = new GameState(newBoard);
            _validator = new Validator(strategy);
            _commandManager = new CommandManager();
            _inputHandler = new InputHandler();
        }

        /// <summary>
        /// Metodă apelată de UI când se dă click pe o celulă.
        /// </summary>
        public void HandleCellClick(int row, int col)
        {
            if (_state.IsVictory) return;

            Cell cell = _state.Board.GetCell(row, col);
            CellState nextState = _inputHandler.GetNextState(cell.State);

            // Dacă utilizatorul vrea să pună o coroană, verificăm prin Validatorul tău
            if (nextState == CellState.Crown)
            {
                if (!_validator.IsMoveValid(_state.Board, row, col))
                {
                    _state.Mistakes++;
                    _state.Score -= 10;
                    // Putem alege să nu permitem mutarea sau să o marcăm ca eroare
                }
            }

            // Executăm mutarea folosind MoveCommand-ul tău
            MoveCommand command = new MoveCommand(_state.Board, row, col, nextState);
            _commandManager.ExecuteCommand(command);

            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            // O logică simplă de victorie: numărul de coroane totale 
            // trebuie să fie egal cu (N coroane per rând) * (Dimensiune Board)
            int totalRequired = _strategy.GetRequiredCrowns() * _state.Board.Size;
            int currentCrowns = 0;

            for (int r = 0; r < _state.Board.Size; r++)
            {
                for (int c = 0; c < _state.Board.Size; c++)
                {
                    if (_state.Board.GetCell(r, c).State == CellState.Crown)
                        currentCrowns++;
                }
            }

            if (currentCrowns == totalRequired)
            {
                _state.IsVictory = true;
            }
        }

        public void Undo() => _commandManager.Undo();
        public void Redo() => _commandManager.Redo();
    }
}