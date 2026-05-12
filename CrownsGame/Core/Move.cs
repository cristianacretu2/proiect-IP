
// clasa este pt a retine starile celulei

namespace CrownsGame.Core
{
    public class Move
    {
        private Position _pos;
        private CellState _previousState;
        private CellState _newState;

        public Position Pos 
        { 
            get { return _pos; }
        }

        public CellState PreviousState
        {
            get { return _previousState; }
        }
        public CellState NewState
        {
            get { return _newState; }
        }

        public Move( Position pos, CellState previousState, CellState newState)
        {
            _pos = pos;
            _previousState = previousState;
            _newState = newState;
        }
    }
}