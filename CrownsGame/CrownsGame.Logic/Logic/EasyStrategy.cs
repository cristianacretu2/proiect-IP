/*
 * Scopul fisierului: Implementeaza strategia de dificultate scazuta (Easy) pentru configurarea jocului.
 * Autor: 
 */

namespace CrownsGame.Logic
{
    /// <summary>
    /// Implementeaza interfata IGameStrategy pentru a defini regulile specifice nivelului Easy.
    /// Acest nivel presupune o tabla de dimensiuni mici si un numar redus de coroane per grup.
    /// </summary>
    public class EasyStrategy : IGameStrategy
    {
        /// <summary>
        /// Determina numarul de coroane necesare per rand, coloana si regiune pentru nivelul Easy.
        /// </summary>
        /// <returns>Valoarea 1, reprezentand o singura coroana obligatorie per grup.</returns>
        public int GetRequiredCrowns()
        {
            return 1;
        }

        /// <summary>
        /// Stabileste dimensiunea tablei de joc pentru dificultatea scazuta.
        /// </summary>
        /// <returns>Valoarea 8, generand un grid de 8x8 celule.</returns>
        public int GetBoardSize()
        {
            return 8; 
        }

        /// <summary>
        /// Returneaza numele oficial al nivelului de dificultate.
        /// </summary>
        /// <returns>Sirul de caractere "Easy".</returns>
        public string GetDifficultyName()
        {
            return "Easy";
        }
    }
}