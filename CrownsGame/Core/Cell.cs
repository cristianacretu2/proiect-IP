namespace CrownsGame.Core
{
    public enum CellState
    {
        Empty,
        Marked,
        Crown
    }

    public class Cell
    {
        private Position _pos;
        private CellState _state;

        // numar care este regiunea din care apartine celula respectiva
        // exista mai multe regiuni de diferite culori 
        private int _regionId; 

        public Position Pos
        {
            get { return _pos; }
        }

        public int RegionId
        {
            get { return _regionId; }
        }

        public CellState State
        {
            get 
            { 
                return _state; 
            }
            set 
            { 
                _state = value; 
            }
        }

        public Cell(Position pos, int regionId)
        {
            _pos = pos;
            _regionId = regionId;
            _state = CellState.Empty;
        }
        
    }
}