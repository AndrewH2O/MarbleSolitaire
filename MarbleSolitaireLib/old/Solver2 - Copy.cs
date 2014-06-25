#define TEST
//#undef TEST
#define STORAGE
//#undef STORAGE



//define TEST sets public accessors for testing lower level granularity functionality
//#define #undef makes it more explicit comment out undef to set TEST and uncomment to ignore
//if we had just #define commented easier to miss
//

using MarbleSolCommonLib.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{

    
/*
    * Notes
    * 
    */
    [Flags]
    public enum LoadState { Start = 1, Win = 2, Current = 4 };

    public class Solver2
    {
        readonly int NUMBER_GAME_POSITIONS = 33; //as used by current game positions
        readonly int STORAGE_BIT_LENGTH = 33 + 9 + 6;
        readonly int MODEL_INDEX_COUNT = 7 * 7;
        readonly int GAME_INDEX_COUNT = 3 + 3 + 7 + 7 + 7 + 3 + 3;
        readonly int NON_LEGAL = -1;
        readonly char TOKEN_SET_CHAR = '1';
        readonly char TOKEN_EMPTY_CHAR = '0';
        readonly int TOKEN_SET_VALUE = 1;
        readonly int TOKEN_EMPTY_VALUE = 0;
        
        readonly ushort DEFAULT_PIECE_COUNT = 32;
        
        public readonly int ERROR = -9;

        //no literal for ushort, math op ushort-(ushort)1 requires a cast therefore lookup 
        //next value from an array: next value = current value - 1 
        //so index 3 gives a value of 2
        ushort[] NEXT_USHORT = { 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
        
        
        ulong _startBoard = 0;
        
        public ulong StartBoard
        {
            get { return _startBoard; }
            private set { _startBoard = value; }
        }

        ulong _nextBoard = 0;

        public ulong NextBoard
        {
            get { return _nextBoard; }
        }

        ulong _currentBoard = 0;

        public ulong CurrentBoard
        {
            get { return _currentBoard; }
            private set { _currentBoard = value; }
        }

        ulong _winBoard = 0;

        public ulong WinBoard
        {
            get { return _winBoard; }
            private set { _winBoard = value; }
        }

        ushort _piecesCount = 0;

        public ushort PiecesCount
        {
            get { return _piecesCount; }
            private set { _piecesCount = value; }
        }

        bool _isSolution = false;

        public bool IsSolution
        {
            get { return _isSolution; }
        }

        bool _enumerateAll = false;

        public bool EnumerateAll
        {
            get { return _enumerateAll; }
            set { _enumerateAll = value; }
        }

        int _solutionCount;

        public int SolutionCount
        {
            get { return _solutionCount; }
        }

#if STORAGE
        RotateFlip _rf;
        StorageController _sc;
#endif
        
        MoveController _moveController;
        PiecesController _piecesController;
        Mapper _mapper;

        ulong[] _solutionProgress;


        StorageBitPacker _sbp; 

        public Solver2(ICandidates candidates)
        {
            initialise(candidates);
        }

        void initialise(ICandidates candidates)
        {
            _sbp = new StorageBitPacker();
            _mapper = new Mapper(candidates);
            _moveController = new MoveController(_mapper, candidates, _sbp);
            _piecesController = new PiecesController(DEFAULT_PIECE_COUNT);
#if STORAGE
            _rf = new RotateFlip(_mapper);
            //_sc = new StorageController();
            _sc = new StorageController(_rf);
#endif
            Reset();
        }

        public void Reset()
        {
            _enumerateAll = false;
            _solutionCount = 0;
            _currentBoard = 0;
            _countIterations = 0;
            
            _piecesCount = 0;
            _solutionProgress = new ulong[DEFAULT_PIECE_COUNT];
            _numberOfPiecesAtStart = 0;

        }

        
        /// <summary>
        /// Overload of LoadBoard 
        /// To set start use the Start flag , which will set both start and current boards.
        /// The Current flag just loads a current board and the Win flag allows assignment to the 
        /// to the winState
        /// </summary>
        /// <param name="valueBoard">board as a list of integers</param>
        /// <param name="state">state flag</param>
        public void LoadBoard(List<int> valueBoard, LoadState state)
        {
            int index;
            ushort piecesCount = 0;
            ulong board=0;
            
            if (valueBoard.Count == MODEL_INDEX_COUNT)
            {
                for (int i = 0; i < MODEL_INDEX_COUNT; i++)
                {
                    index = _mapper.GetModelToGameByIndex(i);
                    if (index != NON_LEGAL && valueBoard[i] == TOKEN_SET_VALUE)
                    {
                        setBit(index, ref board);
                        piecesCount++;
                    }
                }
            }

            if (valueBoard.Count == GAME_INDEX_COUNT)
            {
                for (int i = 0; i < GAME_INDEX_COUNT; i++)
                {
                    if (valueBoard[i] == TOKEN_SET_VALUE)
                    {
                        setBit(i, ref board);
                        piecesCount++;
                    }
                }
            }
            updateBoardFromLoad(state,board,piecesCount);
            
        }

        /// <summary>
        /// Overload of LoadBoard 
        /// To set start use the Start flag , which will set both start and current boards.
        /// The Current flag just loads a current board and the Win flag allows assignment to the 
        /// to the winState
        /// </summary>
        /// <param name="valueBoard">board as a list of integers</param>
        /// <param name="state">state flag</param>
        public void LoadBoard(string valueBoard, LoadState state)
        {
            int index;
            ushort piecesCount = 0;
            ulong board = 0;
            valueBoard = StorageBitPacker.CleanString(valueBoard);
            char[] cb = valueBoard.ToCharArray();
            if (cb.Length == MODEL_INDEX_COUNT)
            {
                for (int i = 0; i < MODEL_INDEX_COUNT; i++)
                {
                    index = _mapper.GetModelToGameByIndex(i);
                    if (index != NON_LEGAL && valueBoard[i] == TOKEN_SET_CHAR)
                    {
                        setBit(index, ref board);
                        piecesCount++;
                    }
                }
            }

            if (cb.Length == GAME_INDEX_COUNT)
            {
                for (int i = 0; i < GAME_INDEX_COUNT; i++)
                {
                    if (valueBoard[i] == TOKEN_SET_CHAR)
                    {
                        setBit(i, ref board);
                        piecesCount++;
                    }
                }
            }

            updateBoardFromLoad(state, board, piecesCount);
        }

        /// <summary>
        /// Load board as 33 bits representing legal positions on the board
        /// </summary>
        /// <param name="board">ulong board</param>
        /// <param name="state">state flag</param>
        public void LoadBoard(ulong board, LoadState state)
        {
            ushort piecesCount = 0;

            for (int i = 0; i < GAME_INDEX_COUNT; i++)
            {
                if (testBit(i,board))
                {
                    piecesCount++;
                }
            }

            updateBoardFromLoad(state, board, piecesCount);
        }

        private void updateBoardFromLoad(LoadState state, ulong board, ushort piecesCount)
        {
            if ((state & LoadState.Current) == LoadState.Current)
            {
                _currentBoard = board;
                _piecesCount = piecesCount;
                _piecesController = new PiecesController(_piecesCount);
                
                initSolutionProgress(_piecesCount);      
            }
            
            if ((state & LoadState.Start) == LoadState.Start)
            {
                _currentBoard = board;
                _startBoard = board;
                _piecesCount = piecesCount;
                _piecesController = new PiecesController(_piecesCount);
                _numberOfPiecesAtStart = _piecesCount;
                initSolutionProgress(_piecesCount);               
            }
            
            if ((state & LoadState.Win) == LoadState.Win)
            {
                _winBoard = board;
            }

        }

        private void initSolutionProgress(ushort _piecesCount)
        {
            _solutionProgress = new ulong[_piecesCount];
        }

        private void updateSolutionProgress(ulong data, ushort piecesCount)
        {
            _solutionProgress[piecesCount-1] = data;
        }

        
        /// TODO tidy up + add init
        ulong _piecesCountPacked = 0;
        ulong _moveIDPacked = 0;
        ulong _boardPacked = 0;
        ulong _storageBits = 0;
        
        /// <summary>
        /// Pack board moveID and piecesCount into storageBits packed ulong format
        /// </summary>
        /// <param name="board">board as ulong</param>
        /// <param name="moveId">nswe(0-3) * 100 + pieceIndex</param>
        /// <param name="piecesCount">pieces count</param>
        /// <returns>storageBits</returns>
        private ulong updateData(ulong board, ushort moveId, ushort piecesCount)
        {
            //lookup packed data
            _piecesController.GetPiecesStorageBitByIndex(piecesCount, ref _piecesCountPacked);
            _moveController.GetStorageBitsByMoveId(moveId, ref _moveIDPacked);
            //use sbp to update and pack into stargeBits
            _sbp.UpdateAll(ref _storageBits, board, _moveIDPacked, _piecesCountPacked);
            
            return _storageBits;
        }
        

        bool _isDone = false;
        
        ushort _numberOfPiecesAtStart = 0;

        ulong[] _boards;
        public void Solve()
        {
            _isDone = false;
#if STORAGE
            _boards = new ulong[8];
            //_sc = new StorageController();
            _sc = new StorageController(_rf);
#endif
            _countIterations = 0;
            initSolutionProgress(_piecesCount);
            if (!_enumerateAll)
            {
                dfs(_currentBoard, _piecesCount);
            }
            else
            {
                dfs_enumAll(_currentBoard, _piecesCount);
                displayStorage();
            }


            Debug.WriteLine("total dfs: {0}", _countIterations);
            
            Debug.WriteLine("total in storage: {0}", _sc.TotalSeenInStorage);
            Debug.WriteLine("total in solution count: {0}", _solutionCount);
            //Debug.WriteLine("total numberOfsolutions: {0}", _sc.NumberOfSolutions);
        }

        private void displayStorage()
        {
            int piecesCount = 4;
            List<ulong> listByPieceCount = _sc.DisplayBoardsInStoreForAGivenPiecesCount(piecesCount);
            if(listByPieceCount.Count>0)
            {
                Debug.WriteLine("Board with piece count of {0}:",piecesCount);
                foreach (var item in listByPieceCount)
                {
                   Debug.WriteLine("Board {0}",item);
                }
            }

            byte contents = 0;
            ulong board = 153;
            _sc.DisplayContentsOfStoreByBoardIndex(board, ref contents);
            Debug.WriteLine("Content board {0} is: {1}", board,contents);
            board = 153 - 1;
            _sc.DisplayContentsOfStoreByBoardIndex(board, ref contents);
            Debug.WriteLine("Content board {0} is: {1}", board, contents);
        }

#if TEST
        int _moveLimit = 0;
        
        public void Solve(int moveLimit)
        {
            _moveLimit = moveLimit;
            Solve();
        }
#endif
        
        
        int _countIterations = 0;
        ushort _currentMove = 0;
        
        
        ushort _nextPiecesCount = 0;
        /// <summary>
        /// Depth first search
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="piecesCount"></param>
        private void dfs(ulong currentBoard, ushort piecesCount)
        {
            _countIterations++;//for info only
            //if(_countIterations%1000==0) Debug.WriteLine("iterations {0}", _countIterations);
#if TEST
            //each time move is made recurse to new level in search
            //levels search cannot exceed number of pieces at start less
            //single piece at the end of the game;
            if (bailOnMoveLimit(piecesCount)) return;
            //if (_countIterations >= 10000000) return;
            
#endif
            if (piecesCount == 1 && currentBoard == _winBoard)
            {
                _isSolution = true;
                _isDone = true;
                _solutionCount++;
                DisplaySolution();
                return;
                //if enumerate all defer to CheckIsKnown
            }

#if STORAGE
            //_rf.GetRotationsFlipsForBoard(currentBoard,_boards);
            //if(_sc.CheckIsKnown(_boards)) return;
            //if (piecesCount>2 && _sc.CheckIsKnown(currentBoard, piecesCount)) return;
            if (_sc.CheckIsKnown(currentBoard, piecesCount))return;
            
#endif

            FindAvailableMoves(currentBoard, piecesCount);
            if (_movesStack.Count == 0) return;//no more moves available so backtrack
            
            //still here? yes then we have moves get one
            Stack<ushort> _currentMoves = _movesStack.Pop();
            while (!_isDone && _currentMoves.Count > 0)
            {
                
                _currentMove = _currentMoves.Pop();
                _nextBoard = makeMove(currentBoard, _moveController.Moves[_currentMove]);
                //_nextPiecesCount = NEXT_USHORT[piecesCount];
                _nextPiecesCount = (ushort)(piecesCount-1);
                updateSolutionProgress(
                    updateData(currentBoard, _currentMove, piecesCount), 
                    _nextPiecesCount);

                dfs(_nextBoard, _nextPiecesCount);
            }

        }

        
        bool _isSolnSeen = false;
        
        private void dfs_enumAll(ulong currentBoard, ushort piecesCount)
        {
            _countIterations++;//for info only
            //if(_countIterations%1000==0) Debug.WriteLine("iterations {0}", _countIterations);

            _isSolution = false;
            
            
            //only runs first time descend down to a solution thereafter 
            //marked off in storage
            if (piecesCount == 1 && currentBoard == _winBoard)
            {
                _isSolution = true;
                updateStorageWithSolution();
                _isSolnSeen = true;
                _solutionCount++;
                //if (!_enumerateAll)
                //{
                //    _isDone = true;
                //    _solutionCount++;
                //    DisplaySolution();
                //    return;
                //}
                
                //if enumerate all defer to CheckIsKnown
            }

#if STORAGE
            //_rf.GetRotationsFlipsForBoard(currentBoard,_boards);
            //if(_sc.CheckIsKnown(_boards)) return;
            //if (piecesCount>2 && _sc.CheckIsKnown(currentBoard, piecesCount)) return;
            if (_sc.CheckIsKnown_enumAll(currentBoard, piecesCount, out _isSolnSeen ))
            {
                if (_isSolnSeen)
                {
                    updateStorageWithSolution(piecesCount,currentBoard);
                    _solutionCount++;
                }
                return;
            }
#endif

            FindAvailableMoves(currentBoard, piecesCount);
            if (_movesStack.Count == 0) return;//no more moves available so backtrack

            //still here? yes then we have moves get one
            Stack<ushort> _currentMoves = _movesStack.Pop();
            while (_currentMoves.Count > 0)
            {
                _currentMove = _currentMoves.Pop();
                _nextBoard = makeMove(currentBoard, _moveController.Moves[_currentMove]);
                //_nextPiecesCount = NEXT_USHORT[piecesCount];
                _nextPiecesCount = (ushort)(piecesCount - 1);
                
                updateSolutionProgress(currentBoard, piecesCount);
                //Debug.WriteLine("_solnProg: board: {0} pieces: {1}", currentBoard, piecesCount);
                dfs_enumAll(_nextBoard, _nextPiecesCount);
            }
        }

        private void updateStorageWithSolution()
        {
            _sc.UpdateWithSolution(_solutionProgress);
            if(_enumerateAll)Array.Clear(_solutionProgress, 0, _solutionProgress.Length);
        }

        private void updateStorageWithSolution(ushort piecesCount, ulong currentBoard)
        {
            _sc.UpdateWithSolution(_solutionProgress, piecesCount, currentBoard);
            if (_enumerateAll) Array.Clear(_solutionProgress, 0, _solutionProgress.Length); 
        }

#if TEST
        private bool bailOnMoveLimit(ushort piecesCount)
        {
            if (_moveLimit!=0 && _moveLimit == _numberOfPiecesAtStart - piecesCount)
            {
                _isDone = true;
                return true;
            }
            return false;
        }
#endif

        private bool isGameStateKnown(ulong _nextBoard, ushort _currentMove)
        {
            //search storage 
            return false;
        }

        public void DisplaySolution()
        {
            ///TODO display soln
            //throw new NotImplementedException();
        }


        #region pending delete
        /*
        ///TODO refactor away into storage class
        /// <summary>
        /// Sets value of bits in storageBits format so the result will be 
        /// properly displaced bits of a ulong aligned using an appropriate mask
        /// to store game board, moveId and number of pieces
        /// 0-32 board 1 bit = 1 position; 
        /// 33-41 moveID 0 to 332 where d * 100 + index 
        ///     where d is nswe direction values 0-3 padded by 100 and 
        ///     index is a position on the gameboard 0-32. 
        ///     so 332 requires 9 bits of storage
        /// 42-47 count of pieces values 0-32 which requires 6 bits of storage
        /// </summary>
        /// <param name="mask">mask of significant bits</param>
        /// <param name="storageBits">ulong storage bits</param>
        private void setStorageBitsValue(ulong mask, ushort value, ref ulong storageBits)
        {
            //calc start index
            int startIndexSignificantBits = -1;
            int countSignificantBits = 0;
            for (int i = 0; i < STORAGE_BIT_LENGTH; i++)
            {
                if (testBit(i, mask))
                {
                    if (startIndexSignificantBits == -1) startIndexSignificantBits = i;
                    countSignificantBits++;
                }
                else
                {
                    //we are done. All set bits contiguous
                    if (countSignificantBits > 0) break;
                }
            }
            
            storageBits = 0;
            for (int i = 0; i < countSignificantBits; i++)
            {
                if (_solverBoard.IsSet(i, value))
                {
                    _solverBoard.setBit(startIndexSignificantBits + i, ref storageBits);
                }
            }
            
        }*/
        #endregion


        Stack<Stack<ushort>> _movesStack = new Stack<Stack<ushort>>();
        Stack<ushort> _availableMoves = new Stack<ushort>();
        MoveSolver2 _ms2 = new MoveSolver2();
        public void FindAvailableMoves(ulong currentBoard, ushort pieceCount)
        {
            //_availableMoves = new Queue<SolverMove>();
            _availableMoves = new Stack<ushort>();
            int piecesFound=0;
            int numberMoves = 0;
            ushort moveID = 0;
            for (int i = 0;  piecesFound<pieceCount && i < NUMBER_GAME_POSITIONS; i++)
            {
                if (testBit(i, currentBoard))
                {
                    piecesFound++;
                    numberMoves = _moveController.MoveLookup[i].Length;

                    for (int j = 0; j < numberMoves; j++)
                    {
                        moveID = _moveController.MoveLookup[i][j];
                        _ms2 = _moveController.Moves[moveID];
                        if ((currentBoard & _ms2.MaskMove) == _ms2.PreMove)
                        {
                            _availableMoves.Push(moveID);
                        }
                    }
                }
            }
            if (_availableMoves.Count > 0)
            {
                //_movesStack.Enqueue(_availableMoves);
                _movesStack.Push(_availableMoves);
            }
        }

        public int AvailableMovesFound()
        {
            if (_availableMoves != null )
            {
                return _availableMoves.Count;
            }
            else
            {
                return ERROR;
            }
        }

#if TEST
        public ulong MakeMove(ulong currentBoard, int moveID)
        {
            MoveSolver2 move = _moveController.Moves[moveID];
            return makeMove(currentBoard, move);
        }

        public ulong UpdateData(ulong board, ushort moveId, ushort piecesCount)
        {
            return updateData(board, moveId, piecesCount);
        }
#endif



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
            if (!validate(index)) return;
            bits |= (1ul << index);
        }

        public void clearBit(int index, ref UInt64 bits)
        {
            if (!validate(index)) return;
            bits &= ~(1ul << index);
        }

        private ulong makeMove(ulong currentBoard, MoveSolver2 move)
        {
            return (currentBoard & ~move.MaskMove) | move.PostMove;
        }

        //private bool isMovePossible(ulong currentBoard, MoveSolver2 move)

        protected bool validate(int index)
        {
            return (index >= 0 && index < NUMBER_GAME_POSITIONS) ? true : false;
            //if (index > MAX_SIZE) return false;
            //return ((index >= 0 && index < _maxValueLegalPositions) ? true : false);
        }

        
    }
}
