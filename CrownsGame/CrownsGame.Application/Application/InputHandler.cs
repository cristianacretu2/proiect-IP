/*
 * Scopul fisierului: Defineste logica de manipulare a inputului pentru a asigura rotirea starilor celulelor.
 * Autor: Sebastian Mihai Lungu
 */

using CrownsGame.Core;

namespace CrownsGame.Application
{
    /// <summary>
    /// Clasa responsabila pentru gestionarea interactiunilor utilizatorului cu elementele de joc.
    /// Aceasta interpreteaza evenimentele de tip click si decide cum trebuie sa evolueze starea unei celule.
    /// </summary>
    public class InputHandler
    {
        /// <summary>
        /// Determina starea urmatoare a unei celule intr-un mod ciclic, permitand utilizatorului sa exploreze solutii.
        /// Logica urmeaza fluxul: Liber -> Marcat (X) -> Coroana -> Liber.
        /// </summary>
        /// <param name="currentState">Starea actuala a celulei inainte de a procesa noul input.</param>
        /// <returns>Noua stare (CellState) rezultata in urma interactiunii.</returns>
        public CellState GetNextState(CellState currentState)
        {
            switch (currentState)
            {
                case CellState.Empty:
                    // Utilizatorul doreste sa marcheze celula ca fiind exclusa din solutie.
                    return CellState.Marked; // Primul click pune X
                case CellState.Marked:
                    // Utilizatorul doreste sa incerce plasarea unei coroane in aceasta locatie.
                    return CellState.Crown;  // Al doilea click pune Coroana
                default:
                    // Daca celula avea deja o coroana sau se afla intr-o alta stare, se revine la starea initiala (reset).
                    return CellState.Empty;  // Al treilea click reseteaza
            }
        }
    }
}

