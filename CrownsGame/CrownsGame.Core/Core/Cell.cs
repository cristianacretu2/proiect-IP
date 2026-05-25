/*
 * Scopul fisierului: Defineste componentele elementare ale tablei (celulele) si starile acestora.
 * Autor: Sebastian Mihai Lungu
 */

namespace CrownsGame.Core
{
    /// <summary>
    /// Enumerare pentru starile posibile ale unei celule, reflectand progresul jucatorului.
    /// </summary>
    public enum CellState
    {
        /// <summary> Celula fara nicio marcare. </summary>
        Empty,  
        /// <summary> Celula eliminata vizual de catre jucator. </summary>
        Marked,  
        /// <summary> Celula care contine o coroana valida. </summary>
        Crown    
    }

    /// <summary>
    /// Reprezinta o unitate individuala pe tabla de joc, pastrand apartenenta la o regiune si starea curenta.
    /// </summary>
    public class Cell
    {
        private int _regionId;
        private CellState _state;

        /// <summary>
        /// Identificatorul unic al regiunii colorate. Folosit pentru validarea regulilor de proximitate.
        /// </summary>
        public int RegionId 
        { 
            get { return _regionId; } 
        }

        /// <summary>
        /// Starea logica actuala a celulei.
        /// </summary>
        public CellState State 
        { 
            get { return _state; } 
            set { _state = value; } 
        }

        /// <summary>
        /// Construieste o celula si o asociaza unei regiuni specifice.
        /// </summary>
        /// <param name="regionId">ID-ul regiunii initiale.</param>
        public Cell(int regionId)
        {
            _regionId = regionId;
            _state = CellState.Empty;
        }

        /// <summary>
        /// Modifica manual regiunea celulei. Utilizat in algoritmii de generare a tablei.
        /// </summary>
        /// <param name="newRegionId">Noul identificator de regiune.</param>
        public void InitializeRegion(int newRegionId)
        {
            _regionId = newRegionId;
        }
    }
}