using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{
    public class StorageControllerBits
    {
        ulong[] _storeA;
        ulong[] _storeB;
        
        ulong[] _store;
        ulong _numberUniqueAfterRotationsFlips = 8;
        ulong _sizeOfUnit = 64;
        int _storeIndex = 0;
        ulong _sizeOfStore; 
        
        RotateFlip _rf;
        int[,] _indexesPacked;

        public StorageControllerBits(RotateFlip rf)
        {
            _rf = rf;
            initialise();
        }

        ulong _numberOfUnits;
        ulong _totalSize;
        public ulong TotalSize
        {
            get { return _totalSize; }
            
        }
        private void initialise()
        {
            if (_rf != null)
            {
                _indexesPacked = _rf.IndexesPacked;
            }
            _numberOfUnits =(ulong) ((Math.Pow(2, 33)) / _sizeOfUnit);
            _sizeOfStore = _numberOfUnits / 2;
            _store = new ulong[_numberOfUnits];

            //_storeA = new ulong[_sizeOfStore];
            //_storeB = new ulong[_sizeOfStore];
            _totalSize = _numberOfUnits * _sizeOfUnit;
        }

        public void Reset()
        {
            initialise();
        }

        public ulong TotalSeenInStorage
        {
            get { return _totalSeenInStorage; }
        }

        public bool IsBoardInStore(ulong board)
        {
            _indexOfAStore = board / _sizeOfUnit;
            _indexInStore = (int)(board - _indexOfAStore * _sizeOfUnit);
            if ((_store[_indexOfAStore] & (1ul << (_indexInStore - 1))) != 0)
            {
                return true;
            }
            return false;
        }

        public void AddToStore(ulong board)
        {
            _indexOfAStore = board / _sizeOfUnit;
            _indexInStore = (int)(board - _indexOfAStore * _sizeOfUnit);
            
            _store[_indexOfAStore] |= (1ul << (_indexInStore-1));
        }

        int _rotationsFlips = 8;
        int _bitsLength = 33;
        ulong[] _rfBoards = new ulong[8];
        ushort _piecesCount;
        ulong _totalSeenInStorage = 0;
        bool _isKnown;
        ulong _currentBoard;
        ulong _indexOfAStore;
        int _indexInStore;
        ulong _currentStorageItem;
        
        public bool CheckIsKnown(ulong board, ushort piecesCount)
        {
            
            _indexOfAStore = board / _sizeOfUnit;
            _indexInStore = (int)(board - _indexOfAStore * _sizeOfUnit);
            if ((_store[_indexOfAStore] & (1ul << _indexInStore)) != 0)
            {
                //_isKnown = true;
                _totalSeenInStorage++;
                return true;//no need to check all
            }

            doRotations(board, piecesCount);
            
            //_store[_indexOfAStore][_indexInStore] = SEEN;
            return false;
        }

        private void doRotations(ulong board, ushort piecesCount)
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

                    _rfBoards[1] |= (1ul << _indexesPacked[1, j]);
                    _rfBoards[2] |= (1ul << _indexesPacked[2, j]);
                    _rfBoards[3] |= (1ul << _indexesPacked[3, j]);
                    _rfBoards[4] |= (1ul << _indexesPacked[4, j]);
                    _rfBoards[5] |= (1ul << _indexesPacked[5, j]);
                    _rfBoards[6] |= (1ul << _indexesPacked[6, j]);
                    _rfBoards[7] |= (1ul << _indexesPacked[7, j]);

                    //setBit(_indexesPacked[i, j], ref _rotatedFlippedBits[i]);
                }
            }

            //mark off all rotations and flips
            for (int i = 0; i < _rfBoards.Length; i++)
            {
                _currentBoard = _rfBoards[i];
                _indexOfAStore = _currentBoard / _sizeOfUnit;
                _indexInStore = (int)(_currentBoard - _indexOfAStore * _sizeOfUnit);
                _store[_indexOfAStore] |= (1ul << _indexInStore);
            }
        }

    }
}
