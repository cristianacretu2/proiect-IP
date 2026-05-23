using System.Collections.Generic;

namespace CrownsGame.Logic
{
    /// <summary>
    /// Gestionează istoricul comenzilor pentru a permite operații de Undo și Redo.
    /// </summary>
    public class CommandManager
    {
        private Stack<ICommand> _undoStack;
        private Stack<ICommand> _redoStack;

        public CommandManager()
        {
            _undoStack = new Stack<ICommand>();
            _redoStack = new Stack<ICommand>();
        }

        /// <summary>
        /// Execută o comandă nouă și o adaugă în istoric.
        /// </summary>
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            // O mutare nouă invalidează istoricul de Redo
            _redoStack.Clear();
        }

        /// <summary>
        /// Anulează ultima comandă executată.
        /// </summary>
        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                ICommand command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }

        /// <summary>
        /// Reface ultima comandă anulată.
        /// </summary>
        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                ICommand command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
        }
    }
}