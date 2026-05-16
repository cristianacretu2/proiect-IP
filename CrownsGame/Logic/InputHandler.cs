using CrownsGame.Core;

namespace CrownsGame.Application
{
    /// <summary>
    /// Interpretează interacțiunile utilizatorului și determină schimbarea de stare a celulelor.
    /// </summary>
    public class InputHandler
    {
        /// <summary>
        /// Determină următoarea stare a unei celule bazată pe starea ei actuală.
        /// </summary>
        public CellState GetNextState(CellState currentState)
        {
            switch (currentState)
            {
                case CellState.Empty:
                    return CellState.Marked; // Primul click pune X
                case CellState.Marked:
                    return CellState.Crown;  // Al doilea click pune Coroană
                default:
                    return CellState.Empty;  // Al treilea click resetează
            }
        }
    }
}