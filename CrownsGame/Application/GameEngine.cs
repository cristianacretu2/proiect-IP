using CrownsGame.Core;
using CrownsGame.Logic;
using System.Collections.Generic;

namespace CrownsGame.Application
{
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
            BoardGenerator generator = new BoardGenerator();
            Board newBoard = generator.Generate(strategy);

            _state = new GameState(newBoard);
            _validator = new Validator(strategy);
            _commandManager = new CommandManager();
            _inputHandler = new InputHandler();
        }

        public void HandleCellClick(int row, int col)
        {
            if (_state.IsVictory) return;

            Cell cell = _state.Board.GetCell(row, col);
            CellState nextState = _inputHandler.GetNextState(cell.State);

            // Validăm doar mutările care plasează o coroană
            if (nextState == CellState.Crown)
            {
                if (!_validator.IsMoveValid(_state.Board, row, col))
                {
                    _state.Mistakes++;
                    _state.Score -= 10;
                    // Chiar dacă e invalidă, o lăsăm pe tablă pentru ca jucătorul 
                    // să vadă eroarea, dar CheckWinCondition nu va da Victory.
                }
            }

            MoveCommand command = new MoveCommand(_state.Board, row, col, nextState);
            _commandManager.ExecuteCommand(command);

            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            int requiredPerGroup = _strategy.GetRequiredCrowns();
            int totalRequired = requiredPerGroup * _state.Board.Size;
            int currentCrowns = CountTotalCrowns();

            // Pasul 1: Verificăm cantitatea
            if (currentCrowns == totalRequired)
            {
                // Pasul 2: Verificăm calitatea (dacă toate regulile sunt respectate)
                if (ValidateFullBoard(requiredPerGroup))
                {
                    _state.IsVictory = true;
                    _state.Score += 100; // Bonus de victorie
                }
            }
        }

        private int CountTotalCrowns()
        {
            int count = 0;
            for (int r = 0; r < _state.Board.Size; r++)
                for (int c = 0; c < _state.Board.Size; c++)
                    if (_state.Board.GetCell(r, c).State == CellState.Crown) count++;
            return count;
        }

        /// <summary>
        /// Verifică dacă întreaga tablă respectă regulile jocului.
        /// </summary>
        private bool ValidateFullBoard(int required)
        {
            int size = _state.Board.Size;
            Dictionary<int, int> regionCounts = new Dictionary<int, int>();

            for (int r = 0; r < size; r++)
            {
                int rowCount = 0;
                int colCount = 0;

                for (int c = 0; c < size; c++)
                {
                    // Verificare Rânduri
                    if (_state.Board.GetCell(r, c).State == CellState.Crown) rowCount++;
                    // Verificare Coloane
                    if (_state.Board.GetCell(c, r).State == CellState.Crown) colCount++;

                    // Verificare Regiuni
                    if (_state.Board.GetCell(r, c).State == CellState.Crown)
                    {
                        int regId = _state.Board.GetCell(r, c).RegionId;
                        regionCounts[regId] = regionCounts.GetValueOrDefault(regId, 0) + 1;

                        // Verificare Adiacență (folosim validatorul tău existent)
                        // Temporar scoatem coroana pentru a nu se auto-invalida
                        _state.Board.SetCellState(r, c, CellState.Empty);
                        bool isPosValid = _validator.IsMoveValid(_state.Board, r, c);
                        _state.Board.SetCellState(r, c, CellState.Crown);

                        if (!isPosValid) return false;
                    }
                }

                if (rowCount != required || colCount != required) return false;
            }

            // Verificăm dacă toate regiunile au numărul corect de coroane
            foreach (var count in regionCounts.Values)
            {
                if (count != required) return false;
            }

            return true;
        }

        public void Undo() => _commandManager.Undo();
        public void Redo() => _commandManager.Redo();
    }
}