/*
 * Scopul fisierului: Implementeaza strategia de dificultate medie (Medium) pentru configurarea jocului.
 * Autor: 
 */

namespace CrownsGame.Logic
{
    /// <summary>
    /// Implementeaza interfata IGameStrategy pentru a defini regulile specifice nivelului Medium.
    /// Acest nivel creste complexitatea prin marirea dimensiunii tablei si a numarului de coroane necesare.
    /// </summary>
    public class MediumStrategy : IGameStrategy
    {
        /// <summary>
        /// Determina numarul de coroane necesare per rand, coloana si regiune pentru nivelul Medium.
        /// </summary>
        /// <returns>Valoarea 2, reprezentand doua coroane obligatorii per grup.</returns>
        public int GetRequiredCrowns()
        {
            return 2;
        }

        /// <summary>
        /// Stabileste dimensiunea tablei de joc pentru dificultatea medie.
        /// </summary>
        /// <returns>Valoarea 10, generand un grid de 10x10 celule.</returns>
        public int GetBoardSize()
        {
            return 10; // Un grid de 10x10 pentru Medium
        }

        /// <summary>
        /// Returneaza numele oficial al nivelului de dificultate.
        /// </summary>
        /// <returns>Sirul de caractere "Medium".</returns>
        public string GetDifficultyName()
        {
            return "Medium";
        }
    }
}