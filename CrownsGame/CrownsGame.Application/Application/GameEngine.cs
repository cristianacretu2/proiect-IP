/*
 * Scopul fisierului: Nucleul aplicatiei care coordoneaza interactiunea dintre componentele logice si starea jocului.
 * Autor: 
 */

using CrownsGame.Core;
using CrownsGame.Logic;
using System.Collections.Generic;

namespace CrownsGame.Application
{

    /// <summary>
    /// Motorul principal al jocului care proceseaza actiunile utilizatorului si valideaza conditiile de victorie globale.
    /// </summary>
    public class GameEngine
    {
        private GameState _state;
        private Validator _validator;
        private CommandManager _commandManager;
        private InputHandler _inputHandler;
        private IGameStrategy _strategy;

        /// <summary> Acces catre datele de stare curente pentru a fi afisate in UI. </summary>
        public GameState State { get { return _state; } }

        /// <summary>
        /// Initializeaza motorul, genereaza o tabla noua si pregateste managerii de comenzi si validare.
        /// </summary>
        /// <param name="strategy">Strategia care defineste regulile specifice nivelului ales.</param>
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

        /// <summary>
        /// Gestioneaza logica de schimbare a starii unei celule la click, aplicand penalizari daca mutarea este gresita.
        /// </summary>
        /// <param name="row">Randul celulei apasate.</param>
        /// <param name="col">Coloana celulei apasate.</param>
        public void HandleCellClick(int row, int col)
        {
            if (_state.IsVictory) return;

            Cell cell = _state.Board.GetCell(row, col);
            CellState nextState = _inputHandler.GetNextState(cell.State);

            // Validam doar mutarile care plaseaza coroana.
            if (nextState == CellState.Crown)
            {
                if (!_validator.IsMoveValid(_state.Board, row, col))
                {
                    _state.Mistakes++;
                    _state.Score -= 10;
                    // Se permite plasarea coroanei gresite pentru feedback vizual, dar nu se acorda victoria.
                }
            }
            // Executarea mutarii prin Command Pattern pentru a permite functionalitatea de Undo/Redo.
            MoveCommand command = new MoveCommand(_state.Board, row, col, nextState);
            _commandManager.ExecuteCommand(command);

            CheckWinCondition();
        }

        /// <summary>
        /// Verifica daca numarul total de coroane de pe tabla este corect si, in caz afirmativ, porneste validarea calitativa a regulilor.
        /// </summary>
        private void CheckWinCondition()
        {
            int requiredPerGroup = _strategy.GetRequiredCrowns();
            int totalRequired = requiredPerGroup * _state.Board.Size;
            int currentCrowns = CountTotalCrowns();

            // Pasul 1: Verificam daca numarul de coroane plasate corespunde cu cel necesar pentru dimensiunea tablei.
            if (currentCrowns == totalRequired)
            {
                // Pasul 2: Verificam calitatea (daca toate regulile de rand, coloana si regiune sunt respectate).
                if (ValidateFullBoard(requiredPerGroup))
                {
                    _state.IsVictory = true;
                    _state.Score += 100; // Bonus de victorie
                }
            }
        }

        /// <summary>
        /// Numara iterativ toate celulele de pe tabla care au starea setata pe Crown.
        /// </summary>
        /// <returns>Numarul total de coroane identificate in grila.</returns>
        private int CountTotalCrowns()
        {
            int count = 0;
            for (int r = 0; r < _state.Board.Size; r++)
                for (int c = 0; c < _state.Board.Size; c++)
                    if (_state.Board.GetCell(r, c).State == CellState.Crown) count++;
            return count;
        }

        /// <summary>
        /// Realizeaza o validare complexa a intregii table, verificand constrangerile de rand, coloana, regiune si proximitate.
        /// </summary>
        /// <param name="required">Numarul de coroane obligatoriu per grup (rand/coloana/regiune).</param>
        /// <returns>True daca intreaga configuratie a tablei este valida conform regulilor jocului.</returns>
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
                    // Monitorizarea numarului de coroane pe randul curent.
                    if (_state.Board.GetCell(r, c).State == CellState.Crown) rowCount++;
                    // Monitorizarea numarului de coroane pe coloana curenta.
                    if (_state.Board.GetCell(c, r).State == CellState.Crown) colCount++;

                    // Verificare Regiuni
                    if (_state.Board.GetCell(r, c).State == CellState.Crown)
                    {
                        int regId = _state.Board.GetCell(r, c).RegionId;
                        regionCounts[regId] = regionCounts.GetValueOrDefault(regId, 0) + 1;

                        // Verificarea regulii de adiacenta prin scoaterea temporara a coroanei (pentru a nu se auto-invalida).
                        _state.Board.SetCellState(r, c, CellState.Empty);
                        bool isPosValid = _validator.IsMoveValid(_state.Board, r, c);
                        _state.Board.SetCellState(r, c, CellState.Crown);

                        if (!isPosValid) return false;
                    }
                }

                if (rowCount != required || colCount != required) return false;
            }

            // Verificam dacă toate regiunile au numărul corect de coroane
            foreach (var count in regionCounts.Values)
            {
                if (count != required) return false;
            }

            return true;
        }

        /// <summary>
        /// Solicita managerului de comenzi sa anuleze ultima actiune de mutare efectuata.
        /// </summary>
        public void Undo() => _commandManager.Undo();

        /// <summary>
        /// Solicita managerului de comenzi sa re-execute o actiune care a fost anterior anulata.
        /// </summary>
        public void Redo() => _commandManager.Redo();
    }
}