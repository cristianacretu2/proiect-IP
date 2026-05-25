/*
 * Scopul fisierului: Implementeaza motorul de sugestii care ajuta jucatorul sa gaseasca mutari valide pe termen lung.
 * Autor: Radani Antonia
 */

using CrownsGame.Core;
using CrownsGame.Logic;

namespace CrownsGame.AI
{
    /// <summary>
    /// Clasa responsabila pentru analizarea tablei si furnizarea celei mai bune mutari catre utilizator.
    /// </summary>
    public class HintEngine
    {
        private readonly Validator _validator;
        private readonly MiniSolver _solver;
        private readonly IGameStrategy _strategy;

        /// <summary>
        /// Constructor care injecteaza dependintele necesare pentru validare si rezolvare.
        /// </summary>
        /// <param name="validator">Componenta care verifica regulile imediate ale jocului.</param>
        /// <param name="strategy">Strategia de joc curenta (ex: numarul de coroane necesare).</param>
        public HintEngine(Validator validator, IGameStrategy strategy)
        {
            _validator = validator;
            _strategy = strategy;
            _solver = new MiniSolver(validator, strategy);
        }

        /// <summary>
        /// Cauta in matricea de joc prima celula libera care, odata completata, pastreaza tabla intr-o stare rezolvabila.
        /// </summary>
        /// <param name="board">Instanta curenta a tablei de joc.</param>
        /// <returns>Pozitia sugestiei gasite sau null daca nu exista o mutare sigura.</returns>
        public Position? GetBestHint(Board board)
        {
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    if (board.GetCell(r, c).State == CellState.Empty)
                    {
                        // Se verifica validitatea mutarii atat prin regulile de baza, cat si prin explorarea viitorului.
                        if (_validator.IsMoveValid(board, r, c))
                        {
                            Board temp = board.Clone();
                            temp.SetCellState(r, c, CellState.Crown);
                            
                            if (_solver.IsSolvable(temp))
                            {
                                return new Position(r, c);
                            }
                        }
                    }
                }
            }
            return null; 
        }
    }
}