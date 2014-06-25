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

        int _indexOf360Degree = 0;
        int _rotationsFlips = 8;
        int _bitsLength = 33;
        ulong[] _rfBoards = new ulong[8];
        ushort _piecesCount;
        ulong _totalSeenInStorage = 0;
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

            //do roatations
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
            
            //do rotations
            //for (int i = 0; i < _rotationsFlips; i++)
            //{
            //    _rfBoards[i] = 0;
            //    _piecesCount = 0;
            //    //_rotatedFlippedBits[i] = 0;
            //    if (i == _indexOf360Degree)//360 degree
            //    {
            //        _rfBoards[i] = board;
            //        continue;
            //    }

            //    for (int j = 0; _piecesCount <= piecesCount && j < _bitsLength; j++)
            //    {
            //        //index = _indexesPacked[i,j];
            //        //if (testBit(j, board))
            //        if ((board & (1ul << j)) != 0)
            //        {
            //            _piecesCount++;
            //            _rfBoards[i] |= (1ul << _indexesPacked[i, j]);
            //            //setBit(_indexesPacked[i, j], ref _rotatedFlippedBits[i]);
            //        }
            //    }
                
            //}

            for (int i = 0; i < _rfBoards.Length; i++)
            {
                _currentBoard = _rfBoards[i];
                _indexOfAStore = _currentBoard / _sizeOfUnit;
                _indexInStore = _currentBoard - _indexOfAStore * _sizeOfUnit;
                _store[_indexOfAStore][_indexInStore] = SEEN;
            }
            
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

    }//end StorageController
    
    
    


    
}
