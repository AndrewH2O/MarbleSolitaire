#define TEST
//#undef TEST
#define STORAGE
//#undef STORAGE
#define DFS_SNAPSHOTS
//#undef DFS_SNAPSHOTS
#define SOLN_SNAPSHOTS
//#undef SOLN_SNAPSHOTS

//define TEST sets public accessors for testing lower level granularity functionality
//#define #undef makes it more explicit comment out undef to set TEST and uncomment to ignore
//if we had just #define commented easier to miss
//

using MarbleSolCommonLib.Common;
using MarbleSolitaireLib.Data;
using MarbleSolitaireLib.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{

    
/*
    * Notes
    * 
    */
    

    public class SolverEnum
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





        SaveState _saveState = 0;

        //no literal for ushort, math op ushort-(ushort)1 requires a cast therefore lookup 
        //next value from an array: next value = current value - 1 
        //so index 3 gives a value of 2
        ushort[] NEXT_USHORT = { 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

#if DFS_SNAPSHOTS
        List<ulong> _dfsVisited; 
#endif
#if SOLN_SNAPSHOTS
        List<object> _solutionSnapShots; 
#endif
        ulong _startBoard = 0;
        
        public ulong StartBoard
        {
            get { return _startBoard; }
            private set { _startBoard = value; }
        }

        int _startPiecesCount = 0;

        public int StartPiecesCount
        {
            get { return _startPiecesCount; }
            private set { _startPiecesCount = value; }
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

        int _piecesCount = 0;

        public int PiecesCount
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
        StorageControllerEnum _sc;
#endif
        
        MoveController _moveController;
        PiecesController _piecesController;
        Mapper _mapper;

        ulong[] _solutionProgress;


        StorageBitPacker _sbp; 

        public SolverEnum(ICandidates candidates)
        {
            initialise(candidates);
        }


        ulong _boardMask;
        ulong _piecesMask;
        void initialise(ICandidates candidates)
        {
            _sbp = new StorageBitPacker();
            _boardMask = _sbp.BoardMask;
            _piecesMask = _sbp.PiecesCountMask;
            _mapper = new Mapper(candidates);
            _moveController = new MoveController(_mapper, candidates, _sbp);
            _piecesController = new PiecesController(DEFAULT_PIECE_COUNT);
#if STORAGE
            _rf = new RotateFlip(_mapper);
            //_sc = new StorageController();
            _sc = new StorageControllerEnum(_rf);
#endif
            Reset();
        }

        public void Reset()
        {
            _saveState = 0;
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
            byte piecesCount = 0;
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
            byte piecesCount = 0;
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
            byte piecesCount = 0;

            for (int i = 0; i < GAME_INDEX_COUNT; i++)
            {
                if (testBit(i,board))
                {
                    piecesCount++;
                }
            }

            updateBoardFromLoad(state, board, piecesCount);
        }

        private void updateBoardFromLoad(LoadState state, ulong board, byte piecesCount)
        {
            if ((state & LoadState.Current) == LoadState.Current)
            {
                _currentBoard = board;
                _piecesCount = piecesCount;
                _piecesController = new PiecesController((ushort)_piecesCount);
                
                initSolutionProgress(_piecesCount);      
            }
            
            if ((state & LoadState.Start) == LoadState.Start)
            {
                _currentBoard = board;
                _startBoard = board;
                _piecesCount = piecesCount;
                _startPiecesCount = piecesCount;
                _piecesController = new PiecesController((ushort)_piecesCount);
                _numberOfPiecesAtStart = (ushort)_piecesCount;
                initSolutionProgress(_piecesCount);               
            }
            
            if ((state & LoadState.Win) == LoadState.Win)
            {
                _winBoard = board;
            }

        }

        
        
        /// TODO tidy up + add init
        ulong _piecesCountPacked = 0;
        ulong _moveIDPacked = 0;
        ulong _boardPacked = 0;
        ulong _storageBits = 0;
        
        
        
        ushort _numberOfPiecesAtStart = 0;

        ulong[] _boards;


        

        public void Solve()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
#if STORAGE
            _boards = new ulong[8];
            
            //_sc = new StorageController();
            _sc = new StorageControllerEnum(_rf);
#endif
            _countIterations = 0;
#if SOLN_SNAPSHOTS            
            _solutionSnapShots = new List<object>();
#endif
#if DFS_SNAPSHOTS
            _dfsVisited = new List<ulong>();
#endif         
            
            initSolutionProgress(_piecesCount);
            dfs(_currentBoard, _piecesCount);

            
            sw.Stop();
            Debug.WriteLine("Time taken to enumerate all {0}:", sw.ElapsedMilliseconds);

            Debug.WriteLine("total dfs: {0}", _countIterations);
            Debug.WriteLine("total seen for count of 3: {0}", _totalSeenForAPieceCount);
            Debug.WriteLine("total in storage: {0}", _sc.TotalSeenInStorage);
            Debug.WriteLine("total in solution count: {0}", _solutionCount);
            Debug.WriteLine("start board:");
            Debug.WriteLine(_startBoard);
#if SOLN_SNAPSHOTS
            if ((_saveState & SaveState.ToText) == SaveState.ToText) saveGameStateToTextFile();
            if ((_saveState & SaveState.ToBinary) == SaveState.ToBinary) saveGameStateToBinary(_fileName);
#endif
            
        }

        string _fileName;
        public void Solve(SaveState saveState, string fileName)
        {
            _fileName = fileName;
            _saveState = saveState;
            Solve();
        }
               
        int _countIterations = 0;
        ushort _currentMove = 0;
        int _totalSeenForAPieceCount=0;
        
        int _nextPiecesCount = 0;
        int _stateOfKnown = 0;
        
        /// <summary>
        /// Depth first search
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="piecesCount"></param>
        private void dfs(ulong currentBoard, int piecesCount)
        {

#if STORAGE
            //_stateOfKnown = 0;
            //if (_sc.CheckIsKnown(currentBoard, piecesCount, out _stateOfKnown))
            if (_sc.CheckIsKnown(currentBoard, piecesCount))
            {
                //_stateOfKnown = _sc.KnownState;
                if ((_sc.StateOfKnown & StorageControllerEnum.STOREVALUE_SOLN) != 0)
                {
                    //look at solution progress and add stuff
                    //should be both seen and part of a solution
                    addPartialTreeToStore(currentBoard, piecesCount);
                    return;
                }
                else if ((_sc.StateOfKnown & StorageControllerEnum.STOREVALUE_SEEN) != 0)
                {
                    return;//where SEEN but may not be part of a solution 
                }
            }
#endif
#if TEST
            //if (piecesCount == 3) _totalSeenForAPieceCount++;
            _countIterations++;//for info only
#endif   
#if DFS_SNAPSHOTS
            updateDfsSnapshots(currentBoard, piecesCount);
#endif      

            if (piecesCount == 1 && currentBoard == _winBoard)
            {
                //_isSolution = true;
                updateSolutionProgress(currentBoard, (ulong)piecesCount);
                addSolnToStore(currentBoard,(ulong)piecesCount);
                //DisplaySolution();
                return;
            }


            FindAvailableMoves(currentBoard, piecesCount);
            if (_movesOrganiser.Count == 0) return;//no more moves available so backtrack
            
            //still here? yes then we have moves get one
            //Stack<ushort> _currentMoves = _movesOrganiser.Pop();
            Stack<ulong> _currentMoves = _movesOrganiser.Pop();
            
            while (_currentMoves.Count > 0)
            {
                //_currentMove = _currentMoves.Pop();
                //_currentMove = _currentMoves.Dequeue();
                //_nextBoard = makeMove(currentBoard, _moveController.Moves[_currentMove]);
                _nextBoard = _currentMoves.Pop();
                //_nextPiecesCount = NEXT_USHORT[piecesCount];
                _nextPiecesCount = piecesCount-1;
                updateSolutionProgress(currentBoard,piecesCount);

                dfs(_nextBoard, _nextPiecesCount);
            }

        }

        

                //Stack<Stack<ushort>> _movesOrganiser = new Stack<Stack<ushort>>();
        //Stack<ushort> _availableMoves = new Stack<ushort>();
        Stack<ulong> _availableMoves = new Stack<ulong>();
        Stack<Stack<ulong>> _movesOrganiser = new Stack<Stack<ulong>>();
        //Queue<ushort> _availableMoves = new Queue<ushort>();
        MoveSolver2 _ms2 = new MoveSolver2();

        int _piecesFound = 0;
        int _numberMoves = 0;

        /// <summary>
        /// Used to find available moves. Searches for current pieces and for
        /// each; retrieves the possible moves and if the move can be made (based
        /// on availibility of surrounding pieces and spaces) then the moveID 
        /// is cached. This will be used to lookup the move later and make it.
        /// </summary>
        /// <param name="currentBoard">current board</param>
        /// <param name="pieceCount">pieces count</param>
        public void FindAvailableMoves(ulong currentBoard, int pieceCount)
        {
            //_availableMoves = new Queue<ushort>();
            _availableMoves = new Stack<ulong>();
            _piecesFound=0;
            _numberMoves = 0;
            ushort moveID = 0;
            for (int i = 0;  _piecesFound<pieceCount && i < NUMBER_GAME_POSITIONS; i++)
            {
                //look for current pieces
                //if (testBit(i, currentBoard))
                if((currentBoard & (1ul << i)) != 0)
                {
                    _piecesFound++;
                    _numberMoves = _moveController.MoveLookup[i].Length;

                    for (int j = 0; j < _numberMoves; j++)
                    {
                        moveID = _moveController.MoveLookup[i][j];
                        _ms2 = _moveController.Moves[moveID];
                        if ((currentBoard & _ms2.MaskMove) == _ms2.PreMove)
                        {
                            //_availableMoves.Enqueue(moveID);
                            //_availableMoves.Push(moveID);
                            //make next move here an push on stack previously this was delayed
                            _availableMoves.Push(makeMove(currentBoard, _ms2));
                            //_availableMoves.Push((currentBoard & ~_ms2.MaskMove) | _ms2.PostMove);
                            
                        }
                    }
                }
            }
            if (_availableMoves.Count > 0)
            {
                //_movesOrganiser.Enqueue(_availableMoves);
                _movesOrganiser.Push(_availableMoves);
            }
        }


        private void initSolutionProgress(int _piecesCount)
        {
            _solutionProgress = new ulong[_piecesCount];
        }

        private void updateSolutionProgress(ulong data, ulong piecesCount)
        {
            data |= _sbp.SetPiecesCount((ushort)piecesCount);
            _solutionProgress[piecesCount - 1] = data;
                 
        }

        private void updateSolutionProgress(ulong data, int piecesCount)
        {
            data |= _sbp.SetPiecesCount((ushort)piecesCount);
            _solutionProgress[piecesCount - 1] = data;

        }

        
#if DFS_SNAPSHOTS 
        ulong _dfsValue;
        /// <summary>
        /// used to track dfs progress
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="piecesCount"></param>
        private void updateDfsSnapshots(ulong currentBoard, int piecesCount)
        {
            _dfsValue = 0;
            //shift pieces as it is not in packed format
            _dfsValue = currentBoard;
            _dfsValue = (_dfsValue & ~_piecesMask) | ((ulong)piecesCount << PIECE_PACKING);
            _dfsVisited.Add(_dfsValue);
        }
#endif
        ulong _sbpValue;
        ulong _sbpPiecesCount;
        ulong _sbpBoard;
        int PIECE_PACKING = 33 + 9;///TODO clean up leaky abstractions
        private void addSolnToStore(ulong currentBoard, ulong piecesCount)
        {
            _sbpValue=0;
            _sbpPiecesCount = 0;
#if SOLN_SNAPSHOTS
            _solutionSnapShots.Add("full");
#endif
            for (int i = 0; i < _solutionProgress.Length; i++)
            {
                _sbpValue = _solutionProgress[i];
                if (_sbpValue == 0) break;//poss excl indexes 0 & 1
                
                _sbpBoard = _sbpValue & _boardMask;
                //_sbpBoard = _sbpValue;
                if (!_sc.IsInStore(_sbpBoard)) //board is part of a seen solution?
                {
                    _sbpPiecesCount = (_sbpValue & _piecesMask) >> PIECE_PACKING;
                    //_sbpPiecesCount = (ulong)i + 1;
                    _sc.AddToStore(_sbpBoard, _sbpPiecesCount);
                    _solutionCount++;
                    
#if SOLN_SNAPSHOTS                    
                    _solutionSnapShots.Add(_sbpValue);
#endif                   
                    //_solutionProgress[i] = 0;//clear what we have seen;
                }
            }

            Array.Clear(_solutionProgress, 0, _solutionProgress.Length - 1);
            
        }

        private void addPartialTreeToStore(ulong currentBoard,int piecesCount)
        {
            //_sbpValue = 0;
#if SOLN_SNAPSHOTS
            _solutionSnapShots.Add("partial");
            //_solutionSnapShots.Add(string.Format("(cp {0} cb {1})",piecesCount,currentBoard));
            
#endif

            _sbpValue = _solutionProgress[piecesCount];
            if (_sbpValue == 0) return;    
            //_sbpValue = currentBoard;
            _sbpBoard = _sbpValue & _boardMask;
            //_sbpBoard = _sbpValue;
            //_sbpPiecesCount = (ulong)piecesCount+1;
            if (!_sc.IsInStore(_sbpBoard))//board is part of a seen solution?
            {
                _sbpPiecesCount = (_sbpValue & _piecesMask) >> PIECE_PACKING;
                _sc.AddToStore(_sbpBoard, _sbpPiecesCount);
#if SOLN_SNAPSHOTS
                _solutionSnapShots.Add(_sbpValue);
#endif
                _solutionCount++;
                _solutionProgress[piecesCount] = 0;//clear what we have seen;
            }
            
            if (piecesCount + 1 <= _solutionProgress.Length)
            {
                for (int i = piecesCount + 1; i < _solutionProgress.Length; i++)
                {
                    _sbpValue = _solutionProgress[i];
                    if (_sbpValue == 0) break;
                    //_sbpValue = currentBoard;
                    _sbpBoard = _sbpValue & _boardMask;
                    if (!_sc.IsInStore(_sbpBoard))//board is part of a seen solution?
                    {
                        _sbpPiecesCount = (_sbpValue & _piecesMask) >> PIECE_PACKING;
                        _sc.AddToStore(_sbpBoard, _sbpPiecesCount);
#if SOLN_SNAPSHOTS
                        _solutionSnapShots.Add(_sbpValue);
#endif
                        _solutionCount++;
                    }
                }
            }

            Array.Clear(_solutionProgress, 0, _solutionProgress.Length - 1);
        }
        



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
            
            _piecesCountPacked = _piecesController.PiecesStorageBit[piecesCount - 1];
            //_piecesController.GetPiecesStorageBitByIndex(piecesCount, ref _piecesCountPacked);
            _moveIDPacked = _moveController.Moves[moveId].StorageBitsValueID;
            //_moveController.GetStorageBitsByMoveId(moveId, ref _moveIDPacked);
            //use sbp to update and pack into stargeBits
            _sbp.UpdateAll(ref _storageBits, board, _moveIDPacked, _piecesCountPacked);

            return _storageBits;
        }
        

        /// <summary>
        /// test bit at given index - used without error checking for speed
        /// </summary>
        /// <param name="index"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        bool testBit(int index, UInt64 board)
        {
            if (!validate(index)) return false;
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

#if TEST
        public int AvailableMovesFound()
        {
            if (_availableMoves != null)
            {
                return _availableMoves.Count;
            }
            else
            {
                return ERROR;
            }
        }

        
        
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
#if TEST
#if SOLN_SNAPSHOTS        
        private void saveGameStateToTextFile()
        {
            bool canDisplayBoards = false;

            using (DisplayHelper display = new DisplayHelper())
            {
                display.Add("*******************************************************");
                display.Add("* total dfs: {0}", _countIterations);
                display.Add("* total in storage: {0}", _sc.TotalSeenInStorage);
                display.Add("* total in solution count: {0}", _solutionCount);
                display.Add("* start board: {0}", _startBoard);
                
                displayBoard(_startBoard, display);
                display.Add("*******************************************************");
                display.Add("* Solution snap shots:");
                
                string prefix = string.Empty;
                display.Add(string.Format("{0}","* Full or Partial data added, PiecesCount,  Board"));
                display.Add("*******************************************************");
                foreach (var item in _solutionSnapShots)
                {
                    if (item.GetType() == typeof(string))
                    {
                        if ((string)item == "full")
                        {
                            prefix = " f: ";
                        }
                        else if ((string)item == "partial")
                        {
                            prefix = " p: ";
                        }
                        else
                        {
                            display.Add((string)item);
                        }
                    }
                
                    if (item.GetType() == typeof(ulong))
                    {
                        _sbpPiecesCount = ((ulong)item & _piecesMask) >> PIECE_PACKING;
                        _sbpBoard = (ulong)item & _boardMask;
                        
                        display.Add(string.Format("{0,3}{1,3:D0}{2,3}{3,11:D0}",
                            prefix, _sbpPiecesCount, ":", _sbpBoard));

                        //display.Append(prefix);
                        //display.Append(_sbpPiecesCount.ToString("D"));
                        //display.Append("\t: ");
                        //display.Append(_sbpBoard.ToString("D"));
                        //display.LineBreak();

                        if (canDisplayBoards) displayBoard(_sbpBoard, display);
                    }

                }
                SolverIO<StringBuilder>.SaveText(display.Contents);
                
            }
            
            //Debug.WriteLine("total numberOfsolutions: {0}", _sc.NumberOfSolutions);
        }


        void saveGameStateToBinary(string fileName)
        {
            int piecesCount=32;
            EnumSolutionsDTO dto = new EnumSolutionsDTO(StartPiecesCount);
            foreach (var item in _solutionSnapShots)
            {
                if (item.GetType() == typeof(ulong))
                {
                    _sbpPiecesCount = ((ulong)item & _piecesMask) >> PIECE_PACKING;
                    _sbpBoard = (ulong)item & _boardMask;

                    dto.AppendData(_sbpPiecesCount, _sbpBoard);
                }
            }

            dto.GenerateArrays();
            SolverIO<IEnumSolutionsDTO>.SaveBinary(dto,fileName);
            
        }

#endif//SOLN_SNAPSHOTS

        private void displayBoard(ulong board,DisplayHelper display)
        {
            display.Append("* "); 
            display.LineBreak();
            int[] lineBreaks = new int[] { 2, 2 + 3, 2 + 3 + 7, 2 + 3 + 7 + 7, 2 + 3 + 7 + 7 + 7, 2 + 3 + 7 + 7 + 7 + 3, 2 + 3 + 7 + 7 + 7 + 3 + 3 };
            int[] spacers = new int[] { 0, 3, 27, 30 };
            display.Append("* ");
            for (int i = 0; i < 33; i++)
            {
                
                for (int j = 0; j < spacers.Length; j++)
                {
                    if (i == spacers[j]) display.Append("  ");
                }

                display.Append(((board & (1ul << i)) != 0 ? "1" : "0"));

                for (int j = 0; j < lineBreaks.Length; j++)
                {
                    if (i == lineBreaks[j])
                    {
                        display.LineBreak();
                        display.Append("* ");
                    }
                }
            }
            
            display.LineBreak();
        }

        
#endif     //TEST  
    }
}