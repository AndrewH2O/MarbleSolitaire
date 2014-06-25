#define TEST
//#undef TEST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MarbleSolitaireLib.GameSolver
{
    /// <summary>
    /// A store indexed by board whose contents is
    /// SEEN = 64, or pieces count 2 to 32
    /// </summary>
    public class StorageController
    {

        byte SEEN = 64;
        byte DEFAULT = 0;
        

        //List<byte[]> _store;
        byte[][] _store;
        ulong _totalSizeStore;
        byte _numberOfStores;
        ulong _sizeOfUnit;
        int _uniqueAfterRotationsFlips = 8;
        
        public StorageController()
        {

            initialise();
        }

        public StorageController(RotateFlip rf)
        {
            // TODO: Complete member initialization
            this._rf = rf;
            
            initialise();
        }



        public void Reset()
        {
            initialise();
        }

        int[,] _indexesPacked;
        private void initialise()
        {
            if (_rf != null)
            { 
                _indexesPacked = _rf.IndexesPacked; 
            }
            _sizeOfUnit = (ulong)(Math.Pow(2, 33) / _uniqueAfterRotationsFlips);
            
            _numberOfStores = 8;
            //_store = new StorageUnit[_numberOfStores];
            //_store = new List<byte[]>(_numberOfStores);
            _store = new byte[_numberOfStores][];
            _totalSizeStore = _numberOfStores * (ulong)_sizeOfUnit;
            for (int i = 0; i < _numberOfStores; i++)
            {
                //_store[i] = new StorageUnit((int)_sizeOfUnit);
                //_store.Add(new byte[_sizeOfUnit]);
                _store[i] = new byte[_sizeOfUnit];
                //for (int j = 0; j < (int)_sizeOfUnit; j++)
                //{
                //    _store[i][j] = DEFAULT;
                //}
            }

            _numberOfSolutions = 0;
        }

        ulong _indexOfAStore = 0;
        ulong _indexInStore = 0;
        ulong _currentBoard;
        bool _isKnown = false;
        private RotateFlip _rf;
        
        public bool CheckIsKnown(ulong[] boards)
        {
            _isKnown = false;
            
            for (int i = 0; i < boards.Length; i++)
            {
                _currentBoard = boards[i];
                _indexOfAStore = _currentBoard / _sizeOfUnit;
                _indexInStore = _currentBoard - _indexOfAStore * _sizeOfUnit;

                if (_store[_indexOfAStore][_indexInStore] == SEEN)
                {
                    //_isKnown = true;
                    return true;//no need to check all
                }
                //else
                //{
                //    _store[(int)_indexOfAStore][_indexInStore] = SEEN;
                //}

            }
            //add last one (we only need to store 1 of 8 possible 
            //rotations/flips
            _store[_indexOfAStore][_indexInStore] = SEEN;
            return false;
        }
        
        int _rotationsFlips = 8;
        int _bitsLength = 33;
        ulong[] _rfBoards = new ulong[8];
        ushort _piecesCount;
        ulong _totalSeenInStorage = 0;

        public ulong TotalSeenInStorage
        {
            get { return _totalSeenInStorage; }
        }

        ulong _numberOfSolutions;

        public ulong NumberOfSolutions
        {
            get { return _numberOfSolutions; }
            
        }

        int _indexOf360Degree = 0;
        public bool CheckIsKnown(ulong board, ushort piecesCount)
        {
            //quick check of board
            _indexOfAStore = board / _sizeOfUnit;
            _indexInStore = board - _indexOfAStore * _sizeOfUnit;

            if (_store[_indexOfAStore][_indexInStore] == SEEN)
            {
                //_isKnown = true;
                _totalSeenInStorage++;
                return true;//no need to check all
            }

            //do rotations
            for (int i = 0; i < _rotationsFlips; i++)
            {
                _rfBoards[i] = 0;
                _piecesCount = 0;
                //_rotatedFlippedBits[i] = 0;
                if (i == _indexOf360Degree)//360 degree
                {
                    _rfBoards[i] = board;
                    continue;
                }
                for (int j = 0; _piecesCount <= piecesCount && j < _bitsLength; j++)
                {
                    //index = _indexesPacked[i,j];
                    //if (testBit(j, board))
                    if ((board & (1ul << j)) != 0)
                    {
                        _piecesCount++;
                        _rfBoards[i] |= (1ul << _indexesPacked[i, j]);
                        //setBit(_indexesPacked[i, j], ref _rotatedFlippedBits[i]);
                    }
                }
                //for (int j = 0; j < _bitsLength; j++)
                //{
                //    //index = _indexesPacked[i,j];
                //    //if (testBit(j, board))
                    
                //        _rfBoards[i] |= (1ul << _indexesPacked[i, j]);
                //        //setBit(_indexesPacked[i, j], ref _rotatedFlippedBits[i]);
                    
                //}
            }
            
            _isKnown = false;
            for (int i = 0; i < _rfBoards.Length; i++)
            {
                _currentBoard = _rfBoards[i];
                _indexOfAStore = _currentBoard / _sizeOfUnit;
                _indexInStore = _currentBoard - _indexOfAStore * _sizeOfUnit;

                if (_store[_indexOfAStore][_indexInStore] == SEEN)
                {
                    //_isKnown = true;
                    _totalSeenInStorage++;
                    return true;//no need to check all
                }
                

            }
            //add last one (we only need to store 1 of 8 possible 
            //rotations/flips
            //_store[_indexOfAStore][_indexInStore] = SEEN;

            for (int i = 0; i < _rfBoards.Length; i++)
            {
                _currentBoard = _rfBoards[i];
                _indexOfAStore = _currentBoard / _sizeOfUnit;
                _indexInStore = _currentBoard - _indexOfAStore * _sizeOfUnit;
                _store[_indexOfAStore][_indexInStore] = SEEN;
            }
            return false;
        }

        byte _valueInStore;
        public bool CheckIsKnown_enumAll(ulong board, ushort piecesCount, out bool isSolnKnown)
        {
            //quick check of board
            
            updateRotationsFlips(board, piecesCount, false);
            
            isSolnKnown = _rfASoln;
            return _rfSeen;
        }



        ulong _currentBoardOnSolnUpdate;
        public void UpdateWithSolution(ulong[] _solutionProgress)
        {
            int piecesCount = 0;
            for (byte i = 0; i < _solutionProgress.Length; i++)
            {
                piecesCount = i + 1;
                _currentBoardOnSolnUpdate = _solutionProgress[i];
                if (_currentBoardOnSolnUpdate > 0)
                {
                    updateRotationsFlips(_currentBoardOnSolnUpdate, piecesCount, true);
                    
                    //_indexOfAStore = _currentBoardOnSolnUpdate / _sizeOfUnit;
                    //_indexInStore = _currentBoardOnSolnUpdate - _indexOfAStore * _sizeOfUnit;
                    //_store[_indexOfAStore][_indexInStore] = (byte)(i + 1);//index to pieces count;
                }
            }

#if TEST
            //displayStorageViewer(_solutionProgress, _store, _sizeOfUnit);
#endif

        }

        byte _currentContents = 0;
        bool _rfASoln, _rfSeen;
        ulong[] _indexesOfAStore = new ulong[8];
        ulong[] _indexesInAStore = new ulong[8];
        private void updateRotationsFlips(ulong board,int piecesCount,bool isSolution)
        {
            //do rotations
            for (int i = 0; i < _rotationsFlips; i++)
            {
                _rfBoards[i] = 0;
                _piecesCount = 0;
                //_rotatedFlippedBits[i] = 0;
                if (i == _indexOf360Degree)//360 degree same as original
                {
                    _rfBoards[i] = board;
                    continue;
                }

                for (int j = 0; _piecesCount <= piecesCount && j < _bitsLength; j++)
                {
                    //index = _indexesPacked[i,j];
                    //if (testBit(j, board))
                    if ((board & (1ul << j)) != 0)
                    {
                        _piecesCount++;
                        _rfBoards[i] |= (1ul << _indexesPacked[i, j]);
                        //setBit(_indexesPacked[i, j], ref _rotatedFlippedBits[i]);
                    }
                }

            }

            _rfASoln=false;
            _rfSeen = false;
            for (int j = 0; j < _rfBoards.Length; j++)
            {
                _currentBoard = _rfBoards[j];
                _indexesOfAStore[j] = _currentBoard / _sizeOfUnit;
                _indexesInAStore[j] = _currentBoard - _indexesOfAStore[j] * _sizeOfUnit;
                _currentContents = _store[_indexesOfAStore[j]][_indexesInAStore[j]];
                if (_currentContents > 0 && _currentContents < SEEN)
                {
                    _rfASoln = true;
                    //_rfSeen = true;
                }
                else if (_currentContents == SEEN)
                {
                    _rfSeen = true;
                    return;
                }
            }

            if (_rfASoln)
            {
                for (int j = 0; j < _rfBoards.Length; j++)
                {
                    _store[_indexesOfAStore[j]][_indexesInAStore[j]] = (byte)(piecesCount);
                }
                
                return;
            }

            for (int j = 0; j < _rfBoards.Length; j++)
            {
                _store[_indexesOfAStore[j]][_indexesInAStore[j]] = SEEN;
            }
        }

        //public bool CheckIsKnown(ulong board)
        //{
        //    _indexOfAStore = board / _sizeOfUnit;
        //    _indexInStore = board - _indexOfAStore * _sizeOfUnit;

        //    if (_store[(int)_indexOfAStore][_indexInStore] == SEEN)
        //    {
        //        //_isKnown = true;
        //        return true;//no need to check all
        //    }
        //    else
        //    {
        //        _store[(int)_indexOfAStore][_indexInStore] = SEEN;
        //        return false;
        //    }

            
            
        //}

        /// <summary>
        /// Used to record the board as a starting position for a solution
        /// Used with solver where if we have a solution the previous series of board
        /// positions are stored for that current solution. Here we just mark off
        /// for each stage wihin that solution, each board position will 
        /// already have been seen. There is no record of a particular 
        /// sequence of wins only that for a given board it participates
        /// in a win. To record all sequences we need to find all sequences and assign
        /// a token to each one and use some bit storage mechanism against each board index.
        /// Initially do not know how many solutions there are!
        /// </summary>
        /// <param name="boardAsIndex"></param>
        public void SetAsSolution(int[] boardsInASolution)
        {
            for (int i = 0; i < boardsInASolution.Length; i++)
            {
                
                //_seen[boardsInASolution[i]] = SOLN;
            }
        }

        

        public List<ulong> DisplayBoardsInStoreForAGivenPiecesCount(int piecesCount)
        {
            List<ulong> boards = new List<ulong>();
            for (ulong i = 0; i < _totalSizeStore; i++)
            {
                _indexOfAStore = i / _sizeOfUnit;
                _indexInStore = i - _indexOfAStore * _sizeOfUnit;
                
                if (_store[_indexOfAStore][_indexInStore] == piecesCount)
                {
                    boards.Add(i);
                }
            }

            return boards;
        }

        public bool DisplayContentsOfStoreByBoardIndex(ulong boardIndex, ref byte contents)
        {
            if (boardIndex > 0 && boardIndex < 8589934591)
            {
                _indexOfAStore = boardIndex / _sizeOfUnit;
                _indexInStore = boardIndex - _indexOfAStore * _sizeOfUnit;


                contents = _store[_indexOfAStore][_indexInStore];
                return true;
            }
            else return false;
        }

#if TEST
        StorageViewer _storageViewer;
#endif
        //ulong _temp;

        public void UpdateWithSolution(ulong[] _solutionProgress, ushort piecesCount, ulong currentBoard)
        {
#if TEST
            //displayStorageViewer(_solutionProgress, _store, _sizeOfUnit);
#endif            
            //update store with new search depth tree. The tree below this  current
            //search node has already been marked as leading to a solution so we just
            //mark all the preceding nodes of the search


            //_currentBoardOnSolnUpdate = currentBoard;
            //_indexOfAStore = _currentBoardOnSolnUpdate / _sizeOfUnit;
            //_indexInStore = _currentBoardOnSolnUpdate - _indexOfAStore * _sizeOfUnit;
            ////_temp = _store[_indexOfAStore][_indexInStore];
            //_store[_indexOfAStore][_indexInStore] = (byte)piecesCount;
            int currentPiecesCount = 0;
            for (byte i = (byte)piecesCount; i < _solutionProgress.Length; i++)
            {
                _currentBoardOnSolnUpdate = _solutionProgress[i];
                currentPiecesCount = piecesCount + 1;
                if (_currentBoardOnSolnUpdate > 0)
                {
                    _numberOfSolutions++;
                    updateRotationsFlips(_currentBoardOnSolnUpdate, currentPiecesCount,true);
                }
                else
                {
                    return;
                }
            }
        }
#if TEST
        private void displayStorageViewer(ulong[] _solutionProgress, byte[][] _store, ulong _sizeOfUnit)
        {
            if (_storageViewer == null)
            {
                _storageViewer = new StorageViewer(_solutionProgress.Length);
            }

            //get storageData
            _storageViewer.Update(_solutionProgress, _store, _sizeOfUnit);
            _storageViewer.Display();
        }
#endif
    
    }//end StorageController
    
    
    
#if TEST
        public class StorageDataItemTest
        {
            public ulong Board { get; set; }
            public ushort PiecesCount { get; set; }
            public byte Value { get; set; }

            public override string ToString()
            {
                return "B,P,V;" + Board + "," + PiecesCount + "," + Value;
            }
        }

        public class StorageViewer
        {
            StorageDataItemTest[] _store;

            public StorageDataItemTest this[int index]
            {
                get { return _store[index]; }
                set { _store[index] = value; }
            }

            public StorageViewer(int length)
            {
                _store = new StorageDataItemTest[length];
                for (int i = 0; i < length; i++)
                {
                    _store[i] = new StorageDataItemTest() { 
                        Board = 0,
                        PiecesCount = (ushort)(i + 1), //index to pieces count
                        Value = 0 
                    };
                }
            }

            ulong _indexOfAStore, _indexInStore;
            public void Update(ulong[] solutionProgress, byte[][] store, ulong sizeOfUnit)
            {
                for (byte i = 0; i < solutionProgress.Length; i++)
                {
                    this[i].Board = solutionProgress[i];
                    _indexOfAStore = solutionProgress[i] / sizeOfUnit;
                    _indexInStore = solutionProgress[i] - _indexOfAStore * sizeOfUnit;
                    this[i].Value = store[_indexOfAStore][_indexInStore];
                }
            }

            public void Display()
            {
                foreach (var item in _store)
                {
                    Debug.WriteLine(item);
                }
            }
        }

#endif

    /*
    public class StorageController
    {
        
        
        //max board value all positions set = 2^33 = 8589934591
        
        //const ulong STORAGE_ITEM_LIMIT = 100000000;//avoid cast
        //const int STORAGE_ITEM_LIMIT_INT = 100000000;//avoid cast
        //const int LIMIT = 86;
        //List<StorageItem> Storage = new List<StorageItem>(LIMIT);

        //public StorageController()
        //{
        //    Initilise();
        //}

        //public void Initilise()
        //{
        //    for (int i = 0; i < LIMIT; i++)
        //    {
        //        StorageItem si = new StorageItem(STORAGE_ITEM_LIMIT_INT);

        //        Storage.Add(si); ;
        //    }
        //}

        //int _divResult;
        //public bool Seen(ulong board, int moveIndex)
        //{
        //    _divResult = (int)(board / STORAGE_ITEM_LIMIT);
            
        //    return Storage[_divResult].IsKnownWithAdd(board, moveIndex);
        //}

    }

    public class StorageItem
    {
        int _limit = 0;
        
        ulong[,] _moves; 

        public StorageItem(int limit)
        {
            _limit = limit;
            _moves = new ulong[_limit,2];
            Reset();
        }

        public void Reset()
        {
            for (int i = 0; i < _limit; i++)
            {
                _moves[i,0] = 0;
                _moves[i,1] = 0;
            }
        }

        public bool IsKnownWithAdd(ulong board, int moveIndex)
        {
            if (moveIndex < 64)
            {
                return checkMove(_moves[board, 0],moveIndex);
            }
            else
            {
                return checkMove(_moves[board, 1], moveIndex);
            }
        }

        bool checkMove(ulong moves, int moveIndex)
        {
            if (testBit(moveIndex,moves))
            {
                return true;
            }
            else
            {
                setBit(moveIndex,ref moves);
                return false;
            }
            
        }

        /// <summary>
        /// test bit at given index - used without error checking for speed
        /// </summary>
        /// <param name="index"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        bool testBit(int index, UInt64 board)
        {
            return (((board & (1ul << index)) != 0) ? true : false);
        }

        public void setBit(int index, ref UInt64 bits)
        {
            bits |= (1ul << index);
        }
    }*/
    
}
