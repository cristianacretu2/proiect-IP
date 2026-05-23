/*
 * Scopul fisierului: Structura usoara pentru reprezentarea coordonatelor bidimensionale.
 * Autor: 
 */
namespace CrownsGame.Core
{


    /// <summary>
    /// Structura imuabila pentru reprezentarea unei locatii (rand, coloana) pe grila de joc.
    /// </summary>
    public struct Position
    {
        /// <summary> Indexul randului (0-based). </summary>
        public int Row { get ; }

        /// <summary> Indexul coloanei (0-based). </summary>
        public int Col { get ; }

        /// <summary>
        /// Initializeaza o pozitie fixa.
        /// </summary>
        /// <param name="row">Randul.</param>
        /// <param name="col">Coloana.</param>
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    
}