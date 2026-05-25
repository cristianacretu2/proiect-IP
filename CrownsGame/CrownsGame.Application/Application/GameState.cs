/*
 * Scopul fisierului: Defineste clasa GameState care actioneaza ca un container pentru datele si statisticile unei partide in desfasurare.
 * Autor: Sebastian Mihai Lungu
 */

using CrownsGame.Core;

namespace CrownsGame.Application
{
    /// <summary>
    /// Gestioneaza starea logica si statistica a unui joc, centralizand informatiile despre tabla, scor si erori.
    /// Aceasta clasa este utilizata de motorul de joc pentru a urmari progresul utilizatorului.
    /// </summary>
    public class GameState
    {
        private Board _board;
        private int _score;
        private int _mistakes;
        private bool _isVictory;

        /// <summary>
        /// Referinta catre instanta tablei de joc pe care se efectueaza mutarile in sesiunea curenta.
        /// </summary>
        public Board Board { get { return _board; } }

        /// <summary>
        /// Valoarea numerica a punctajului acumulat, influentata de corectitudinea plasarilor si viteza de raspuns.
        /// </summary>
        public int Score { get { return _score; } set { _score = value; } }

        /// <summary>
        /// Numarul de incercari invalide de a plasa o coroana, conform regulilor de validare imediata.
        /// </summary>
        public int Mistakes { get { return _mistakes; } set { _mistakes = value; } }

        /// <summary>
        /// Indicator boolean care devine true atunci cand toate restrictiile de pe tabla au fost satisfacute.
        /// </summary>
        public bool IsVictory { get { return _isVictory; } set { _isVictory = value; } }

        /// <summary>
        /// Initializeaza o noua stare de joc, setand scorul si greselile la valorile de baza si legand tabla de joc.
        /// </summary>
        /// <param name="board">Obiectul de tip Board care reprezinta structura curenta a puzzle-ului.</param>
        public GameState(Board board)
        {
            _board = board;
            _score = 0;
            _mistakes = 0;
            _isVictory = false;
        }
    }
}