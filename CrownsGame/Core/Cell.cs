namespace CrownsGame.Core
{
    /// <summary>
    /// Enumerare pentru stările posibile ale unei celule pe tabla de joc.
    /// </summary>
    public enum CellState
    {
        Empty,   // Celulă neutră
        Marked,  // Marcată cu "X" de către jucător
        Crown    // Conține o coroană
    }

    /// <summary>
    /// Reprezintă o unitate elementară a tablei de joc.
    /// </summary>
    public class Cell
    {
        private int _regionId;
        private CellState _state;

        /// <summary>
        /// Identificatorul unic al regiunii colorate din care face parte celula.
        /// </summary>
        public int RegionId 
        { 
            get { return _regionId; } 
        }

        /// <summary>
        /// Starea actuală a celulei (Empty, Marked sau Crown).
        /// </summary>
        public CellState State 
        { 
            get { return _state; } 
            set { _state = value; } 
        }

        /// <summary>
        /// Constructor pentru inițializarea celulei cu o regiune specifică.
        /// </summary>
        /// <param name="regionId">ID-ul regiunii atribuite.</param>
        public Cell(int regionId)
        {
            _regionId = regionId;
            _state = CellState.Empty;
        }

        /// <summary>
        /// Permite modificarea ID-ului regiunii în faza de generare a tablei.
        /// </summary>
        /// <param name="newRegionId">Noul ID de regiune.</param>
        public void InitializeRegion(int newRegionId)
        {
            _regionId = newRegionId;
        }
    }
}