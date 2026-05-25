/*
 * Scopul fisierului: Implementeaza strategia de dificultate ridicata (Hard) pentru configurarea jocului.
 * Autor: 
 */

namespace CrownsGame.Logic
{
    /// <summary>
    /// Implementeaza interfata IGameStrategy pentru a defini regulile specifice nivelului Hard.
    /// Acest nivel reprezinta provocarea maxima, utilizand un grid extins si un numar mare de coroane.
    /// </summary>
    public class HardStrategy : IGameStrategy
    {
        /// <summary>
        /// Determina numarul de coroane necesare per rand, coloana si regiune pentru nivelul Hard.
        /// </summary>
        /// <returns>Valoarea 3, reprezentand trei coroane obligatorii per grup.</returns>
        public int GetRequiredCrowns()
        {
            return 3;
        }

        /// <summary>
        /// Stabileste dimensiunea tablei de joc pentru dificultatea ridicata.
        /// </summary>
        /// <returns>Valoarea 14, generand un grid complex de 14x14 celule.</returns>
        public int GetBoardSize()
        {
            return 14; 
        }

        /// <summary>
        /// Returneaza numele oficial al nivelului de dificultate.
        /// </summary>
        /// <returns>Sirul de caractere "Hard".</returns>
        public string GetDifficultyName()
        {
            return "Hard";
        }
    }
}