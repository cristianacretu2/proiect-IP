/*
 * Scopul fisierului: Implementeaza managerul de comenzi responsabil pentru stocarea istoricului mutarilor si gestionarea operatiilor Undo/Redo.
 * Autor: Cretu Cristiana
 */

using System.Collections.Generic;

namespace CrownsGame.Logic
{
    /// <summary>
    /// Gestioneaza istoricul comenzilor pentru a permite operatii de Undo si Redo.
    /// Utilizeaza doua stive (LIFO) pentru a urmari mutarile efectuate si pe cele anulate.
    /// </summary>
    public class CommandManager
    {
        private Stack<ICommand> _undoStack;
        private Stack<ICommand> _redoStack;

        /// <summary>
        /// Constructor care initializeaza stivele de istoric pentru mutari.
        /// </summary>
        public CommandManager()
        {
            _undoStack = new Stack<ICommand>();
            _redoStack = new Stack<ICommand>();
        }

        /// <summary>
        /// Executa o comanda noua, o adauga in stiva de Undo si curata istoricul de Redo.
        /// </summary>
        /// <param name="command">Comanda ce urmeaza a fi executata si salvata in istoric.</param>
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            
            // O mutare noua invalideaza istoricul de Redo pentru a mentine consistenta liniara a starilor.
            _redoStack.Clear();
        }

        /// <summary>
        /// Anuleaza ultima comanda executata, mutand-o in stiva de Redo pentru o eventuala refacere.
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
        /// Reface ultima comanda anulata prin metoda Undo, executand-o din nou si mutand-o inapoi in stiva de Undo.
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