/*
 * Scopul fisierului: Inregistrarea unei schimbari de stare a unei celule pentru gestionarea istoricului.
 * Autor: Sebastian Mihai Lungu
 */

namespace CrownsGame.Core
{
    /// <summary>
    /// Stocheaza datele despre o singura interactiune a utilizatorului cu tabla, permitand revenirea la starea anterioara.
    /// </summary>
    public class Move
    {
        private Position _pos;
        private CellState _previousState;
        private CellState _newState;

        /// <summary>
        /// Pozitia pe tabla unde a avut loc mutarea.
        /// </summary>
        public Position Pos 
        { 
            get { return _pos; }
        }

        /// <summary>
        /// Starea in care se afla celula inainte de interactiune.
        /// </summary>
        public CellState PreviousState
        {
            get { return _previousState; }
        }

        /// <summary>
        /// Starea celulei dupa finalizarea mutarii.
        /// </summary>
        public CellState NewState
        {
            get { return _newState; }
        }

        /// <summary>
        /// Creeaza un obiect care descrie complet o mutare efectuata.
        /// </summary>
        /// <param name="pos">Coordonatele celulei.</param>
        /// <param name="previousState">Starea veche.</param>
        /// <param name="newState">Starea noua.</param>
        public Move( Position pos, CellState previousState, CellState newState)
        {
            _pos = pos;
            _previousState = previousState;
            _newState = newState;
        }
    }
}