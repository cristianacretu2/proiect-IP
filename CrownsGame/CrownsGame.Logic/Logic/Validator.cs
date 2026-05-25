/*
 * Scopul fisierului: Implementeaza regulile de validare ale jocului pentru a asigura respectarea constrangerilor de plasare a coroanelor.
 * Autor: Cretu Cristiana
 */

using CrownsGame.Core;
using System.Collections.Generic;

namespace CrownsGame.Logic
{
    /// <summary>
    /// Clasa responsabila pentru verificarea legalitatii mutarilor conform regulilor jocului: 
    /// limite pe rand, coloana, regiune si restrictia de adiacenta.
    /// </summary>
    public class Validator
    {
        private readonly int _requiredCrowns;

        /// <summary>
        /// Initializeaza validatorul folosind un numar fix de coroane permise per grup.
        /// </summary>
        /// <param name="requiredCrowns">Numarul maxim de coroane per rand/coloana/regiune.</param>
        public Validator(int requiredCrowns)
        {
            _requiredCrowns = requiredCrowns;
        }

        /// <summary>
        /// Initializeaza validatorul extragand numarul de coroane necesare din strategia de joc curenta.
        /// </summary>
        /// <param name="strategy">Instanta strategiei care defineste dificultatea jocului.</param>
        public Validator(IGameStrategy strategy)
        {
            _requiredCrowns = strategy.GetRequiredCrowns();
        }

        /// <summary>
        /// Verifica daca plasarea unei coroane la coordonatele specificate respecta toate regulile active pe tabla de joc.
        /// </summary>
        /// <param name="board">Instanta tablei de joc verificate.</param>
        /// <param name="row">Indexul randului unde se doreste mutarea.</param>
        /// <param name="col">Indexul coloanei unde se doreste mutarea.</param>
        /// <returns>True daca mutarea este valida, altfel False.</returns>
        public bool IsMoveValid(Board board, int row, int col)
        {
            // Verificam daca celula vizata nu are deja o coroana.
            if (board.GetCell(row, col).State == CellState.Crown) return false;

            // Verificarea tuturor constrangerilor logice.
            if (!CheckAdjacency(board, row, col)) return false;
            if (!CheckRowLimit(board, row)) return false;
            if (!CheckColumnLimit(board, col)) return false;

            int regionId = board.GetCell(row, col).RegionId;
            if (!CheckRegionLimit(board, regionId)) return false;

            return true;
        }

        /// <summary>
        /// Verifica daca exista alte coroane in cele 8 celule adiacente pozitiei curente (inclusiv pe diagonala).
        /// </summary>
        /// <param name="board">Tabla de joc.</param>
        /// <param name="row">Randul celulei de verificat.</param>
        /// <param name="col">Coloana celulei de verificat.</param>
        /// <returns>True daca nu exista coroane in vecinatate.</returns>
        private bool CheckAdjacency(Board board, int row, int col)
        {
            for (int r = row - 1; r <= row + 1; r++)
            {
                for (int c = col - 1; c <= col + 1; c++)
                {
                    // Validam limitele tablei si ignoram celula curenta in cautarea vecinilor.
                    if (r >= 0 && r < board.Size && c >= 0 && c < board.Size && !(r == row && c == col))
                    {
                        if (board.GetCell(r, c).State == CellState.Crown) return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Verifica daca numarul de coroane de pe randul specificat nu a atins deja limita maxima permisa.
        /// </summary>
        /// <param name="board">Tabla de joc.</param>
        /// <param name="row">Randul investigat.</param>
        /// <returns>True daca se mai poate plasa o coroana pe acest rand.</returns>
        private bool CheckRowLimit(Board board, int row)
        {
            int count = 0;
            for (int c = 0; c < board.Size; c++)
                if (board.GetCell(row, c).State == CellState.Crown) count++;
            
            return count < _requiredCrowns;
        }

        /// <summary>
        /// Verifica daca numarul de coroane de pe coloana specificata nu a atins deja limita maxima permisa.
        /// </summary>
        /// <param name="board">Tabla de joc.</param>
        /// <param name="col">Coloana investigata.</param>
        /// <returns>True daca se mai poate plasa o coroana pe aceasta coloana.</returns>
        private bool CheckColumnLimit(Board board, int col)
        {
            int count = 0;
            for (int r = 0; r < board.Size; r++)
                if (board.GetCell(r, col).State == CellState.Crown) count++;
            
            return count < _requiredCrowns;
        }

        /// <summary>
        /// Verifica daca regiunea colorata din care face parte celula mai permite adaugarea unei noi coroane.
        /// </summary>
        /// <param name="board">Tabla de joc.</param>
        /// <param name="regionId">ID-ul regiunii verificate.</param>
        /// <returns>True daca limita regiunii nu a fost depasita.</returns>
        private bool CheckRegionLimit(Board board, int regionId)
        {
            int count = 0;
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    var cell = board.GetCell(r, c);
                    if (cell.RegionId == regionId && cell.State == CellState.Crown)
                        count++;
                }
            }
            return count < _requiredCrowns;
        }
    }
}