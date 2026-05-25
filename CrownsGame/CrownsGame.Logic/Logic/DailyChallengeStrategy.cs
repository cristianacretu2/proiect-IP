/*
 * Scopul fisierului: Implementeaza strategia specifica pentru modul Daily Challenge, 
 * oferind o configuratie rapida si compacta pentru sesiuni de joc contra-cronometru.
 * Autor: Cretu Cristiana
 */

namespace CrownsGame.Logic
{
    /// <summary>
    /// Implementeaza interfata IGameStrategy pentru modul de joc Daily Challenge.
    /// Aceasta strategie utilizeaza o tabla redusa (5x5) pentru a permite rezolvarea rapida a mai multor puzzle-uri intr-un timp limitat.
    /// </summary>
    public class DailyChallengeStrategy : IGameStrategy
    {
        /// <summary>
        /// Stabileste dimensiunea tablei de joc pentru modul Daily Challenge.
        /// </summary>
        /// <returns>Valoarea 5, generand un grid compact de 5x5 celule.</returns>
        public int GetBoardSize() => 5;

        /// <summary>
        /// Determina numarul de coroane necesare per grup pentru acest mod de joc.
        /// </summary>
        /// <returns>Valoarea 1, facilitand o rezolvare dinamica.</returns>
        public int GetRequiredCrowns() => 1;

        /// <summary>
        /// Returneaza numele oficial al modului de dificultate pentru afisarea in UI.
        /// </summary>
        /// <returns>Sirul de caractere "Daily Challenge".</returns>
        public string GetDifficultyName() => "Daily Challenge";
    }
}