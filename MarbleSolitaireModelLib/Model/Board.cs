using CrossCuttingLib.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MarbleSolitaireModelLib.Model
{
    public abstract class Board : IBoard
    {
        protected int _length { get; private set; }
        

        public List<int> Start { get; set; }

        protected List<int> _boardItems;
        
        public List<int> BoardItems
        {
            get { return _boardItems; }
            protected set { _boardItems = value; }
        }

        protected int _countPieces;

        public int CountPieces
        {
            get { return _countPieces; }
            set { _countPieces = value; }
        }

        protected int _countPiecesStart;

        public int CountPiecesStart
        {
            get { return _countPiecesStart; }
            set { _countPiecesStart = value; }
        }
        
        protected bool _hasStart;
        
        public bool HasStart
        {
            get { return _hasStart; }
            private set { _hasStart = value; }
        }


        public int TokenIllegalPosition { get; protected set; }
        public int TokenIsSpace { get; protected set; }
        public int TokenHasPiece { get; protected set; }
        public int TokenUnknown { get; protected set; }

        

        public Board(List<int> layout)
        {
            _hasStart = false;
            TokenIllegalPosition = -1;
            TokenHasPiece = 1;
            TokenIsSpace = 0;
            TokenUnknown = -9;

            if (validateBoard(layout))
            {
                this._boardItems = layout;
                this._length = _boardItems.Count;
            }
        }

        protected bool validateBoard(List<int> board)
        {
            if (board == null || board.Count == 0)
            {
                Errors.Log("Board", "01001");
                return false;
            }
            else
            {
                return true;
            }
        }

        internal void validateStart(List<int> start)
        {
            if (!validateBoard(start))throw new Exception("error with start board setup");
            
            _hasStart = true;
            
        }

        protected void ClearBoard()
        {
            for (int i = 0; i < _boardItems.Count; i++)
            {
                _boardItems[i] = 0;
            }
        }

        public abstract void SetupStart(List<int> start);//bool hasStart=true optional
        public abstract int[] GetListOfJumpedCandidates(int index);
        public abstract int[] GetListOfTargetCandidates(int index);
        public abstract int[] GetListOfSourceCandidates(int index);
        public abstract void MakeMove(int start, int jumped, int target);
        public abstract void UnMakeMove(int start, int jumped, int target);
        public abstract bool CheckMove(int start, int jumped, int target);
        public abstract bool CheckUndoMove(int start, int jumped, int target);
    }
}
