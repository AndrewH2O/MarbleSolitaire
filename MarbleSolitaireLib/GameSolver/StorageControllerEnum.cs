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
    public class StorageControllerEnum
    {
        
        //state
        public const byte STOREVALUE_UNKNOWN = 0;
        public const byte STOREVALUE_SEEN = 64;
        public const byte STOREVALUE_SOLN = 128;
        public const byte STOREVALUE_PIECES_COUNT_MASK = 63;

        //note values 1 to 32 incl are piecesCount
        //uses STOREVALUE flags
        public byte _storeValue;


        //List<byte[]> _store;
        byte[][] _store;
        ulong _totalSizeStore;
        byte _numberOfStores;
        ulong _sizeOfUnit;
        int _uniqueAfterRotationsFlips = 8;
        
        public StorageControllerEnum()
        {

            initialise();
        }

        public StorageControllerEnum(RotateFlip rf)
        {
            // TODO: Complete member initialization
            this._rf = rf;
            
            initialise();
        }


        public void Reset()
        {
            initialise();
        }

        int[,] _rfIndexesPackedArray;
        private void initialise()
        {
            if (_rf != null)
            { 
                _rfIndexesPackedArray = _rf.IndexesPacked; 
            }
            _sizeOfUnit = (ulong)(Math.Pow(2, 33) / _uniqueAfterRotationsFlips);
            
            _numberOfStores = 8;
            //_store = new StorageUnit[_numberOfStores];
            //_store = new List<byte[]>(_numberOfStores);
            _store = new byte[_numberOfStores][];
            _totalSizeStore = _numberOfStores * (ulong)_sizeOfUnit;
            for (int i = 0; i < _numberOfStores; i++)
            {
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
        
        
        int _indexOf360Degree = 0;
        int _rotationsFlips = 8;
        int _bitsLength = 33;
        ulong[] _rfBoards = new ulong[8];
        ushort _piecesCount;
        ulong _totalSeenInStorage = 0;
        byte _valueInStore;

        public int StateOfKnown;


        /// <summary>
        /// Check if we have seen this board, if so we only find out later if it
        /// is part of a solution in which case mark it as so next time.
        /// STOREVALUE_SEEN and STOREVALUE_SOLN bits keep track of what is known
        /// and when. These are stored in the knownState, which reports back to the caller.
        /// A SEEN board must precede a SOLN board
        /// The first significant bits of known State store are used to store piecesCount
        /// which is a value.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="piecesCount"></param>
        /// <param name="knownState"></param>
        /// <returns></returns>
        public bool CheckIsKnown(ulong board, int piecesCount, out int knownState)
        {
            //quick check of board
            _indexOfAStore = board / _sizeOfUnit;
            _indexInStore = board - _indexOfAStore * _sizeOfUnit;

            _valueInStore = _store[_indexOfAStore][_indexInStore];
            //if SEEN rtn true and carry on or
            knownState = 0;
            _isKnown = false;
            if ((_valueInStore & STOREVALUE_SEEN) != 0)
            {
                //report back knownState
                knownState |= STOREVALUE_SEEN;
                _isKnown = true;
            }

            //have we seen this?
            if ((_valueInStore & STOREVALUE_SOLN) != 0)
            {
                //report back
                
                knownState |= STOREVALUE_SOLN;
                _isKnown = true;
                
            }
            //solution report back

            if (_isKnown)
            {
                return true;
            }
            else
            {
                addRotationsFlips(board, piecesCount, AddToStoreAsEnum.SEEN);
                return false;
            }
        }

        public bool CheckIsKnown(ulong board, int piecesCount)
        {
            //quick check of board
            _indexOfAStore = board / _sizeOfUnit;
            _indexInStore = board - _indexOfAStore * _sizeOfUnit;

            _valueInStore = _store[_indexOfAStore][_indexInStore];
            //if SEEN rtn true and carry on or
            StateOfKnown = 0;
            _isKnown = false;
            if ((_valueInStore & STOREVALUE_SEEN) != 0)
            {
                //report back knownState
                StateOfKnown |= STOREVALUE_SEEN;
                _isKnown = true;
            }

            //have we seen this?
            if ((_valueInStore & STOREVALUE_SOLN) != 0)
            {
                //report back

                StateOfKnown |= STOREVALUE_SOLN;
                _isKnown = true;

            }
            //solution report back

            if (_isKnown)
            {
                return true;
            }
            else
            {
                addRotationsFlips(board, piecesCount, AddToStoreAsEnum.SEEN);
                return false;
            }
            
        }



        enum AddToStoreAsEnum { SEEN, SOLN };

        
        
        /// <summary>
        /// updates store for all rotations and flips
        /// assumes that ordering of the rf indexes packed has indexOf360 as the 
        /// first item; If rf packed Indexes order changes this breaks;
        /// </summary>
        /// <param name="board"></param>
        /// <param name="piecesCount"></param>
        /// <param name="state"></param>
        private void addRotationsFlips(ulong board, int piecesCount, AddToStoreAsEnum state)
        {
            //do rotations
            
            _rfBoards[0] = board;//indexOf360degree which is same as current

            _rfBoards[1] = 0;
            _rfBoards[2] = 0;
            _rfBoards[3] = 0;
            _rfBoards[4] = 0;
            _rfBoards[5] = 0;
            _rfBoards[6] = 0;
            _rfBoards[7] = 0;

            _piecesCount = 0;
            for (int j = 0; _piecesCount <= piecesCount && j < _bitsLength; j++)
            {
                //index = _indexesPacked[i,j];
                //if (testBit(j, board))
                if ((board & (1ul << j)) != 0)
                {
                    _piecesCount++;
                    
                    _rfBoards[1] |= (1ul << _rfIndexesPackedArray[1, j]);
                    _rfBoards[2] |= (1ul << _rfIndexesPackedArray[2, j]);
                    _rfBoards[3] |= (1ul << _rfIndexesPackedArray[3, j]);
                    _rfBoards[4] |= (1ul << _rfIndexesPackedArray[4, j]);
                    _rfBoards[5] |= (1ul << _rfIndexesPackedArray[5, j]);
                    _rfBoards[6] |= (1ul << _rfIndexesPackedArray[6, j]);
                    _rfBoards[7] |= (1ul << _rfIndexesPackedArray[7, j]);
                    
                    //setBit(_indexesPacked[i, j], ref _rotatedFlippedBits[i]);
                }
            }
            
            switch (state)
            {
                case AddToStoreAsEnum.SEEN:
                    addWithRFToStoreAsSeen(_rfBoards, piecesCount);
                    break;
                case AddToStoreAsEnum.SOLN:
                    addWithRFToStoreAsSoln(_rfBoards, piecesCount);
                    break;
                default:
                    break;
            }
            
            
        }

        int _valueToStore;
        public void AddToStore(ulong board, ulong piecesCount)
        {
            //_currentBoard = board;
            //_indexOfAStore = _currentBoard / _sizeOfUnit;
            //_indexInStore = _currentBoard - _indexOfAStore * _sizeOfUnit;
            //_valueToStore = 0;
            //_valueToStore = (int)piecesCount | STOREVALUE_SOLN;
            //_store[_indexOfAStore][_indexInStore] = (byte)_valueToStore;
            addRotationsFlips(board, (int)piecesCount,AddToStoreAsEnum.SOLN);
        }


        private void addWithRFToStoreAsSeen(ulong[] boards, int piecesCount)
        {
            for (int i = 0; i < boards.Length; i++)
            {
                _currentBoard = boards[i];
                _indexOfAStore = _currentBoard / _sizeOfUnit;
                _indexInStore = _currentBoard - _indexOfAStore * _sizeOfUnit;
                _valueToStore = _store[_indexOfAStore][_indexInStore];
                _valueToStore |= (_valueToStore & ~STOREVALUE_SEEN) | STOREVALUE_SEEN;
                
                if(_store[_indexOfAStore][_indexInStore]==0)_totalSeenInStorage++;//avoid double counting symmetry
                
                _store[_indexOfAStore][_indexInStore] = (byte)_valueToStore;
            }  
        }

        private void addWithRFToStoreAsSoln(ulong[] boards, int piecesCount)
        {
            _valueToStore = 0;
            //note clear SEEN flags and replace with piecesCount and soln flag
            _valueToStore = piecesCount | STOREVALUE_SOLN;
            
            for (int i = 0; i < boards.Length; i++)
            {
                _currentBoard = boards[i];
                _indexOfAStore = _currentBoard / _sizeOfUnit;
                _indexInStore = _currentBoard - _indexOfAStore * _sizeOfUnit;
                
                _store[_indexOfAStore][_indexInStore] = (byte)_valueToStore;
            }
        }

        public bool IsInStore(ulong board)
        {
            _indexOfAStore = board / _sizeOfUnit;
            _indexInStore = board - _indexOfAStore * _sizeOfUnit;
            _valueInStore = _store[_indexOfAStore][_indexInStore];
            if ((_valueInStore & STOREVALUE_SOLN) != 0) return true;
            //if (_valueInStore > 0 && _valueInStore < STOREVALUE_SEEN) return true;

            return false;

        }

        public ulong TotalSeenInStorage
        {
            get { return _totalSeenInStorage; }
        }

        ulong _numberOfSolutions;

        public ulong NumberOfSolutions
        {
            get { return _numberOfSolutions; }
            
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
                _store[i] = new StorageDataItemTest()
                {
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
        

    
}
