using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{
    public class EnumSolutionsOld
    {
        const int NUMBER_GAME_POSITIONS = 33;
        ulong _totalSizeStore;
        ulong _currentBoard;
        ulong _currentRFBoard;
        List<ulong>[] _boards = new List<ulong>[33];
        ulong _countBoardsStored = 0;
        public List<ulong>[] Boards
        {
            get { return _boards; }
        }
        
        StorageControllerBits _sc;
        RotateFlip _rf;
        int _numberRotationsFlips;
        //33 positions
        int[,] _indexesPacked;
        ulong[] _rotatedFlippedBits;
        byte _currentPiecesCount;
        
        public EnumSolutionsOld(RotateFlip rf)
        {
            _rf = rf;
            _numberRotationsFlips=_rf.RotationsFlips;
            _indexesPacked = _rf.IndexesPacked;
            _rotatedFlippedBits = new ulong[_numberRotationsFlips];
            _sc = new StorageControllerBits(rf);
            
            
            
            for (int i = 0; i < _boards.Length; i++)
            {
                if (i > 0 && i < 33)
                {
                    _boards[i] = new List<ulong>();
                }
                else
                {
                    _boards[i] = null;
                }
            }

            ulong highestBoard = 8589934590;
            _totalSizeStore = _sc.TotalSize;
            
            for (ulong i = 0; i < _totalSizeStore; i++)
            {
                if (i == 0) continue;
                _currentBoard = i;
                _currentPiecesCount = 0;

                if (_sc.IsBoardInStore(_currentBoard)) continue;
                               
                
                for (int j = 0; j < _numberRotationsFlips; j++)
                {
                    if (j == 0)
                    {
                        _sc.AddToStore(_currentBoard);
                        
                        _countBoardsStored++;
                    }
                    else
                    {
                        _currentRFBoard = 0;
                        for (int k = 0; k < NUMBER_GAME_POSITIONS; k++)
                        {
                            if (testBit(k, _currentBoard))
                            {
                                //update piecesCount only one time
                                if (j == 1) _currentPiecesCount++;
                                setBit(_indexesPacked[j, k], ref _currentRFBoard);
                            }
                        }
                        _sc.AddToStore(_currentRFBoard);
                        _countBoardsStored++;
                    }
                }
                
                _boards[_currentPiecesCount].Add(_currentBoard);

            }

            
        }

        
        
        private byte getPiecesCountFromBoard(ulong board)
        {
            _currentPiecesCount=0;
            for (int i = 0; i < NUMBER_GAME_POSITIONS; i++)
            {
                if (testBit(i, board))
                {
                    _currentPiecesCount++;
                }
            }
            return _currentPiecesCount;
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

        public void clearBit(int index, ref UInt64 bits)
        {

            bits &= ~(1ul << index);
        }
    }
}
