/*
 * Scopul fisierului: Defineste contractul pentru strategiile de dificultate, permitand algoritmilor de generare si validare sa fie independenti de nivelul ales.
 * Autor: Cretu Cristiana
 */

namespace CrownsGame.Logic
{
    /// <summary>
    /// Interfata pentru nivelurile de dificultate ale jocului: Easy, Medium, Hard.
    /// Design pattern folosit: Strategy - permite schimbarea regulilor de configurare a tablei la runtime.
    /// </summary>
    public interface IGameStrategy
    {
        /// <summary>
        /// Returneaza denumirea textuala a nivelului de dificultate pentru afisarea in interfata grafica.
        /// </summary>
        /// <returns>Un sir de caractere reprezentand numele dificultatii (ex: "Easy").</returns>
        string GetDifficultyName();
        
        /// <summary>
        /// Determina numarul de coroane necesare per grup (rand/coloana/regiune) pentru configuratia curenta.
        /// </summary>
        /// <returns>Numarul intreg de coroane obligatorii.</returns>
        int GetRequiredCrowns();

        /// <summary>
        /// Determina dimensiunea gridului patrat: de exemplu 8 pentru Easy, 10 pentru Medium, 14 pentru Hard.
        /// </summary>
        /// <returns>Numarul de celule pe o latura a tablei de joc.</returns>
        int GetBoardSize();
    }
    
}