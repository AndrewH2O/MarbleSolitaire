
using MarbleSolCommonLib.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{
    
    
    
    public class Solver:ISolver
    {
        readonly int NONLEGAL = -1;
        readonly UInt16 DIMENSION = 4;
        
        SolverBoard _solverBoard;
        List<SolverNode> _nodes = new List<SolverNode>();

        ulong _currentBoard;

        public ulong CurrentBoard
        {
            get { return _currentBoard; }
        }

        public List<SolverNode> Nodes
        {
            get { return _nodes; }
        }

        public Solver(SolverBoard board)
        {
            _solverBoard = board;
        }
        List<int> _indexMapper = new List<int>();
        public void InitialiseMoves(ICandidates candidates)
        {
            int count = -1;
            foreach (var indexModel in candidates
                .EnumerateNodesByIndex(x => x.Content != NONLEGAL))
            {
                buildMoves(
                    candidates.GetListOfSourceCandidates(indexModel),
                    candidates.GetListOfJumpedCandidates(indexModel),
                    candidates.GetListOfTargetCandidates(indexModel),
                    ++count,
                    indexModel
                    );
                _indexMapper.Add(indexModel);
            }

            

        }

        
        int _numberPiecesAtStart;
        /// <summary>
        /// Load start position as 7 by 7 with 1 where a piece is set and 0 
        /// for all others
        /// </summary>
        /// <param name="start"></param>
        public void LoadStart(List<int> start)
        {
            _solverBoard.SetCurrentPositions(start);
            _currentBoard = _solverBoard.BoardCurrent;
            _numberPiecesAtStart=_solverBoard.CountPieces(CurrentBoard);
        }

        

        /// <summary>
        /// Ensures we check for correct winning state
        /// Also poosible to have a single piece remaining in the wrong position
        /// eg board ulong of 8 only has 1 piece on it.
        /// </summary>
        /// <param name="win"></param>
        public void SetWinningState(List<int> win)
        {
            _solverBoard.SetWinPosition(win);
        }

        int _moveLimit;
        int _countIterations;

        public int CountIterations
        {
            get { return _countIterations; }
            set { _countIterations = value; }
        }



        public void Solve()
        {
            Solve(-1);
        }

        public void Solve(int moveLimit)
        {
            MemoManager.Initialise(_nodes);
            
            _isDone = false;
            if (moveLimit == -1) moveLimit = int.MaxValue-2;
            _moveLimit = moveLimit;
            _countIterations = 0;
            primeCacheProgress();
            _isSolution = false;
            dfs(_currentBoard, _numberPiecesAtStart);
            

            Debug.WriteLine(this._solverBoard.DisplayBoard(_nextBoard, "nextboard after "+moveLimit+" moves", 7));
        }

        private void primeCacheProgress()
        {
            _storeProgress = new List<StoreProgressItem>();
            for (int i = 0; i < _numberPiecesAtStart; i++)
            {
                _storeProgress.Add(new StoreProgressItem());
                
            }
        }

        SolverMove _currentMove;
        ulong _nextBoard;

        public ulong NextBoard
        {
            get { return _nextBoard; }
            
        }
        
        int _nextNumberPieces;
        bool _isDone;
        bool _isSolution;

        public bool IsSolution
        {
            get { return _isSolution; }
        }

        
        private void dfs(ulong currentBoard,int numberPieces)
        {
            _countIterations++;
            if (_countIterations > _moveLimit)
            {
                _isDone = true;
                return;
            }
            if (numberPieces == 1&&currentBoard==_solverBoard.BoardWin)
            {
                _isDone = true;
                _isSolution = true;
                DisplaySolution();
                return;
            }
            FindAvailableMoves(currentBoard);
            //Queue<SolverMove> _currentMoves = _movesStack.Dequeue();
            if (!(_movesStack.Count > 0)) return;
            Stack<SolverMove> _currentMoves = _movesStack.Pop();
            while (!_isDone && _currentMoves.Count>0)
            {

                //_currentMove = _currentMoves.Dequeue();
                _currentMove = _currentMoves.Pop();
                _nextBoard=makeMove(currentBoard,_currentMove);
                _nextNumberPieces = numberPieces - 1;

                updateStore(currentBoard, _currentMove, numberPieces, _nextNumberPieces, _nextBoard);

                if (haveSeen(_nextBoard, _currentMove.Direction, _currentMove.Index))
                {
                    if (_nextNumberPieces == 1 && _nextBoard == _solverBoard.BoardWin)
                    {
                        _isDone = true;
                        _isSolution = true;
                        DisplaySolution();
                        return;
                    }
                    if (_countIterations > _moveLimit)
                    {
                        _isDone = true;
                        return;
                    }
                    continue;
                }

                dfs(_nextBoard,_nextNumberPieces);
            }
            
        }

        
        
        private bool haveSeen(ulong currentBoard, int direction, int index)
        {
            //TODO rotations

            StoreSeenItem seenItem = new StoreSeenItem(){ Board=currentBoard, MoveDirection=(direction * 100 + index)};
            return MemoManager.HasSeen(seenItem);
            
        }

        private void updateStore(ulong currentBoard, SolverMove currentMove, int numberPieces, int nextNumberPieces, ulong nextBoard)
        {
            //_storeProgress[numberPieces - 1].Board = currentBoard;
            //_storeProgress[numberPieces - 1].NumberOfPieces = numberPieces;
            //_storeProgress[numberPieces - 1].Move = currentMove;
            _storeProgress[numberPieces - 1]= new StoreProgressItem()
            {
                Board = currentBoard,
                Move = currentMove,
                NumberOfPieces = numberPieces,
                NextNumberOfPieces=nextNumberPieces,
                NextBoard=NextBoard
            };
            if (nextNumberPieces == 1)
            {
                //_storeProgress[0].Board = nextBoard;
                
                //_storeProgress[0].NumberOfPieces = nextNumberPieces;
                _storeProgress[0]= new StoreProgressItem()
                {
                    Board = nextBoard,
                    Move = new SolverMove() { },
                    NumberOfPieces = nextNumberPieces
                    
                };
            }
        }

        private void DisplaySolution()
        {
            Debug.WriteLine("==== solution ====");
            foreach (var item in _storeProgress)
            {
                Debug.WriteLine(item);
            }
        }

        public ulong GetSolutionPart(int index)
        {
            if (index >= 0 && index < _storeProgress.Count)
            {
                return _storeProgress[index].Board;
            }
            else
                return 0;
        }

        List<StoreProgressItem> _storeProgress;  
        

        private ulong makeMove(ulong currentBoard, SolverMove move)
        {
            return (currentBoard & ~move.Mask) | move.PostMove;
        }

        //Queue<SolverMove> _availableMoves;
        Stack<SolverMove> _availableMoves;
        
        
        Stack<Stack<SolverMove>> _movesStack = new Stack<Stack<SolverMove>>();
        
        //Queue<Queue<SolverMove>> _movesStack = new Queue<Queue<SolverMove>>();
        //public Queue<SolverMove> AvailableMoves
        //{
        //    get { return _availableMoves; }
        //}

        public Stack<SolverMove> AvailableMoves
        {
            get { return _availableMoves; }
        }

        public void FindAvailableMoves(ulong currentBoard)
        {
            //_availableMoves = new Queue<SolverMove>();
            _availableMoves = new Stack<SolverMove>();
            if (_nodes.Count == 0) return;
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (testBit(i,currentBoard))
                {
                    IsMovePossible(_nodes[i], currentBoard);
                }
            }
            //_movesStack.Enqueue(_availableMoves);
            if (_availableMoves.Count > 0)
            {
                _movesStack.Push(_availableMoves);
            }
        }

        private bool testBit(int index, ulong board)
        {
            return (((board & (1ul << _indexMapper[index])) != 0) ? true : false);
        }

        

        public string DisplayCurrentBoard()
        {
            return _solverBoard.DisplayBoard(this.CurrentBoard, "Current board of solver", 7);
        }

        private bool IsMovePossible(SolverNode node, ulong board)
        {
            for (UInt16 i = 0; i < DIMENSION; i++)
            {
                //mask is zero then invalid
                if (node.Mask[i]==0)continue;
                //check move using mask and pre move
                if ((board & node.Mask[i]) == node.PreMoves[i])
                {
                    _availableMoves.Push(new SolverMove()
                        {
                            Mask = node.Mask[i],
                            PreMove = node.PreMoves[i],
                            PostMove = node.PostMoves[i],
                            Index = node.Index,
                            Direction = i
                        });
                    //_availableMoves.Enqueue(new SolverMove()
                    //{
                    //    Mask = node.Mask[i],
                    //    PreMove = node.PreMoves[i],
                    //    PostMove = node.PostMoves[i],
                    //    Index = node.Index,
                    //    Direction = i
                    //});
                }
            }

            return false;
        }

        private void buildMoves(int[] source, int[] jumped, int[] target,int index, int indexModel)
        {
            SolverNode node = new SolverNode(DIMENSION);
            node.Index = (UInt16)index;
            node.IndexModel = indexModel;
            UInt64 preMove, maskMove,postMove;
            
            for (int i = 0; i < DIMENSION; i++)
            {
                if (source[i]!=NONLEGAL && jumped[i] !=NONLEGAL && target[i] != NONLEGAL)
                {
                    preMove = 0;
                    maskMove = 0;
                    postMove = 0;
                    _solverBoard.SetPreMove(source[i], jumped[i], target[i], ref preMove);
                    _solverBoard.SetMaskMove(source[i], jumped[i], target[i], ref maskMove);
                    _solverBoard.SetPostMove(source[i], jumped[i], target[i], ref postMove);
                    node.PreMoves[i] = preMove;
                    node.Mask[i] = maskMove;
                    node.PostMoves[i] = postMove;
                }
            }
            _nodes.Add(node);
        }

        public bool GetHint(ref int indexPieceToMoveNext, ref int directionOfMove)
        {
            if (!_isSolution||_storeProgress==null||_storeProgress.Count==0) return false;
            if (_numberPiecesAtStart != _storeProgress.Count) return false;
            indexPieceToMoveNext = _storeProgress[_numberPiecesAtStart-1].Move.Index;
            directionOfMove = _storeProgress[_numberPiecesAtStart-1].Move.Direction;

            return true;
        }

        public ulong GetBestBoardWhereNoSolution()
        {
            if (!this.IsSolution)
            {
                foreach (var item in this._storeProgress)
                {
                    if (item.NumberOfPieces != 0) return item.NextBoard;
                }
            }
            return 0;
        }
    }

    public class SolverNode
    {
        public ulong[] PreMoves { get; set; }
        public ulong[] Mask { get; set; }
        public ulong[] PostMoves { get; set; }
        public ushort Index { get; set; }
        public int IndexModel { get; set; }
        
        public SolverNode(int MaxMoveChoices)
        {
            if (MaxMoveChoices <= 0) 
                throw new ArgumentOutOfRangeException("MaxMoveChoices must be greater than 0");
            PreMoves = new ulong[MaxMoveChoices];
            Mask = new ulong[MaxMoveChoices];
            PostMoves = new ulong[MaxMoveChoices];
        }

        
    }

    public struct SolverMove
    {
        public ulong PreMove { get; set; }
        public ulong Mask { get; set; }
        public ulong PostMove { get; set; }
        public ushort Index { get; set; }
        public ushort Direction  { get; set; }
    }

    public struct StoreProgressItem
    {
        public SolverMove Move { get; set; }
        public int NumberOfPieces { get; set; }
        public ulong Board { get; set; }
        public ulong NextBoard { get; set; }
        public int NextNumberOfPieces { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" b:");
            sb.Append(Board);
            sb.Append(" i:");
            sb.Append(Move.Index);
            sb.Append(" m:");
            sb.Append(Move.Mask);
            sb.Append(" pr:");
            sb.Append(Move.PreMove);
            sb.Append(" po:");
            sb.Append(Move.PostMove);
            sb.Append(" d:");
            sb.Append(Move.Direction);
            sb.Append(" pc:");
            sb.Append(NumberOfPieces);
            sb.Append(" nb:");
            sb.Append(NextBoard);
            sb.Append(" npc:");
            sb.Append(NextNumberOfPieces);
            //sb.Append("\n");
            return sb.ToString();
        }
    }

    
}
