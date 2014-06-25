using CrossCuttingLib.Errors;
using MarbleSolitaireLib.GameSolver;
using MarbleSolitaireModelLib.Model;
using MarbleSolitaireViewModel.Moves;
using MarbleSolitaireViewModel.ViewHelpers;
using MarbleSolitaireViewModel.ViewModel.Moves;
using MarbleSolitaireViewModel.ViewModel.UndoRedo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace MarbleSolitaireViewModel.ViewModel
{

    public class GameViewModel : ViewModelBase,IDisposable
    {
        #region constants

        readonly double MIN_SIDE_LENGTH = 450;
        readonly int ERROR = -1;

        #endregion //constants

        #region fields

        /// <summary>
        /// Maps model and view space
        /// </summary>
        IMapModelViewSpace _mapModelViewSpace;
        
        /// <summary>
        /// Controls moves and sends notifications to this VM
        /// </summary>
        IViewMoveController _moveController;
        
        WinHandler _winHandler;
        HintHandler _hintHandler;
        MoveHandler _moveHandler;
        PiecesCountHandler _piecesCountHandler;


        /// <summary>
        /// Solver
        /// </summary>
        ISolver _solver;

        /// <summary>
        /// A 1 dimensional array representing a 7 by 7 board that marks out the legal board cells
        /// The model coordinates are easy to index 
        /// </summary>
        List<int> _simpleBoardInModelCoords;

        /// <summary>
        /// Manages undo redo stacks
        /// </summary>
        UndoRedoManager<UndoRedoState> _undoRedoManager;





        /// <summary>
        /// Logs errors
        /// </summary>
        IErrorLog _errors;

        //store source viewIndex, jump as ModelIndex target as model index



        /// <summary>
        /// Used to toggle show search animation state
        /// </summary>
        bool _isSearching = false;

        #endregion //fields

        #region properties

        /// <summary>
        /// The piece loacted in a cell
        /// </summary>
        public ObservableCollection<Piece> Pieces
        {
            get { return _moveController.Pieces; }
        }



        #region IsSolving

        private bool _isSolving = false;

        /// <summary>
        /// Is set when solving which may be long running used to direct flow and
        /// alter game state
        /// </summary>
        public bool IsSolving
        {
            get { return _isSolving; }
            set
            {
                if (_isSolving != value)
                {
                    _isSolving = value;
                    RaisePropertyChanged("IsSolving");
                }
            }
        }

        #endregion



        #region IsWin

        private bool _isWin = false;

        /// <summary>
        /// Indicates a win
        /// </summary>
        public bool IsWin
        {
            get { return _isWin; }
            set
            {
                if (_isWin != value)
                {
                    _isWin = value;
                    RaisePropertyChanged("IsWin");
                }
            }
        }

        #endregion

        #region IsHintBtnHidden

        private bool _isHintBtnHidden = false;

        /// <summary>
        /// Display state of the Hint View Object used to toggle visibility
        /// </summary>
        public bool IsHintBtnHidden
        {
            get { return _isHintBtnHidden; }
            set
            {
                if (_isHintBtnHidden != value)
                {
                    _isHintBtnHidden = value;
                    RaisePropertyChanged("IsHintBtnHidden");
                }
            }
        }

        #endregion

        #region ShowHintInfo

        private bool _showHintInfo = true;

        /// <summary>
        /// Shows the results of a Solve operation and toggles View Object that
        /// displays the hint 
        /// </summary>
        public bool ShowHintInfo
        {
            get { return _showHintInfo; }
            set
            {
                if (_showHintInfo != value)
                {
                    _showHintInfo = value;
                    RaisePropertyChanged("ShowHintInfo");
                }
            }
        }

        #endregion

        #region HintIndexNextPieceToMove

        private string _hintIndexNextPieceToMove = string.Empty;

        /// <summary>
        /// Is the index of the next piece to move which is the first step towards
        /// a solution
        /// </summary>
        public string HintIndexNextPieceToMove
        {
            get { return _hintIndexNextPieceToMove; }
            set
            {
                if (_hintIndexNextPieceToMove != value)
                {
                    _hintIndexNextPieceToMove = value;
                    RaisePropertyChanged("HintIndexNextPieceToMove");
                }
            }
        }

        #endregion

        #region HintDirectionPieceToMove

        private string _hintDirectionPieceToMove = string.Empty;

        /// <summary>
        ///Is the direction of the next piece to move which is the first step towards
        ///a solution
        /// </summary>
        public string HintDirectionPieceToMove
        {
            get { return _hintDirectionPieceToMove; }
            set
            {
                if (_hintDirectionPieceToMove != value)
                {
                    _hintDirectionPieceToMove = value;
                    RaisePropertyChanged("HintDirectionPieceToMove");
                }
            }
        }

        #endregion

        #region IsUndoCmdEnabled

        private bool _isUndoCmdEnabled = true;

        /// <summary>
        /// Toggles the undo routed command - activated once there is something to undo
        /// </summary>
        public bool IsUndoCmdEnabled
        {
            get { return _isUndoCmdEnabled; }
            set
            {
                if (_isUndoCmdEnabled != value)
                {
                    _isUndoCmdEnabled = value;
                    RaisePropertyChanged("IsUndoCmdEnabled");
                }
            }
        }

        #endregion

        #region IsRedoCmdEnabled

        private bool _isRedoCmdEnabled = false;

        /// <summary>
        /// Toggles Redo routed command set after an undo action
        /// </summary>
        public bool IsRedoCmdEnabled
        {
            get { return _isRedoCmdEnabled; }
            set
            {
                if (_isRedoCmdEnabled != value)
                {
                    _isRedoCmdEnabled = value;
                    RaisePropertyChanged("IsRedoCmdEnabled");
                }
            }
        }

        #endregion

        #region IsUndoRedoBtnHidden

        private bool _isUndoRedoBtnHidden = false;

        /// <summary>
        /// Toggles UndoRedo Button state
        /// </summary>
        public bool IsUndoRedoBtnHidden
        {
            get { return _isUndoRedoBtnHidden; }
            set
            {
                if (_isUndoRedoBtnHidden != value)
                {
                    _isUndoRedoBtnHidden = value;
                    RaisePropertyChanged("IsUndoRedoBtnHidden");
                }
            }
        }

        #endregion




        #region SideLengthBoard

        private double _sideLengthBoard = 0;

        /// <summary>
        /// Gets or sets the SideLength property. This observable property 
        /// indicates the size of the Gameboard
        /// </summary>
        public double SideLengthBoard
        {
            get { return _sideLengthBoard; }
            set
            {
                if (_sideLengthBoard != value)
                {
                    _sideLengthBoard = value;
                    RaisePropertyChanged("SideLengthBoard");
                }
            }
        }
        #endregion

        #region PiecesCount

        private int _piecesCount = 0;

        /// <summary>
        /// Gets or sets the PiecesCount property
        /// </summary>
        public int PiecesCount
        {
            get { return _piecesCount; }
            set
            {
                if (_piecesCount != value)
                {
                    _piecesCount = value;
                    RaisePropertyChanged("PiecesCount");
                }
            }
        }

        #endregion



        #region CurrentIndex

        int _currentIndex;

        /// <summary>
        /// Gets or sets the CurrentIndex property. This observable property 
        /// indicates currently selected Pieces index.
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                if (_currentIndex != value)
                {
                    _currentIndex = value;
                    _moveController.UpdateSelectedPiece(_currentIndex);
                    RaisePropertyChanged("CurrentIndex");
                }
            }
        }

        #endregion

        #endregion // properties

        #region ctors and initialisation


        //public GameViewModel2():
        //    this(
        //    new GameAttributes(null).GetBoardWithStart(),
        //        new GameAttributes(null).GetSolver2ForSquareBoardWithData(),
        //        null) //new CCEmptyErrorLog()


        /// <summary>
        /// Parameterless ctor as required by a viewmodel - calls ctor overload 
        /// determining what our dependencies should be based on if we are running  
        /// a game or showing the view in the designer - Blend/Designer friendly
        /// </summary>
        public GameViewModel() :
            this(
                 (ISquareBoard)DesignerWorkFlow.Get(DesignerWorkFlow.BOARD),
                 (ISolver)DesignerWorkFlow.Get(DesignerWorkFlow.SOLVER),
                 (IErrorLog)DesignerWorkFlow.Get(DesignerWorkFlow.ERRORLOG)
             )
        { }




        /// <summary>
        /// Ctor - can be called directly
        /// </summary>
        /// <param name="board">Board using model coordinates</param>
        /// <param name="solver">A solver</param>
        /// <param name="errorLog">An error logger</param>
        public GameViewModel(ISquareBoard board, ISolver solver, IErrorLog errorLog)
        {
            initialiseUndoRedo();
            _mapModelViewSpace = new MapModelViewSpace();

            _moveController = new ViewMoveController(board, _mapModelViewSpace);

            wireUpMoveControllerHandlers();

            if (errorLog != null) _errors = errorLog;

            if (solver != null) _solver = solver;
            //used to construct our pieces observable
            _simpleBoardInModelCoords = _moveController.GetBoard();


            //use model co-ordinates for an absolute layout - 
            //the board shapes our piece layout and each
            //one stores its position.

            int viewIndex = -1; //used in mapping between view and model space

            //model layout
            int countOfItemsPerSide = _moveController.GetSide();
            if (SideLengthBoard == 0) SideLengthBoard = MIN_SIDE_LENGTH;
            int margin = (int)(SideLengthBoard * .05);
            int sideLengthPiece = ((int)SideLengthBoard - 2 * margin) / countOfItemsPerSide;


            if (_simpleBoardInModelCoords != null && _simpleBoardInModelCoords.Count > 0)
            {
                for (int i = 0; i < _simpleBoardInModelCoords.Count; i++)
                {
                    ///TODO add auto resize behaviour

                    if (_simpleBoardInModelCoords[i] == _moveController.GetToken(BoardTokens.TokenHasPiece)
                        || _simpleBoardInModelCoords[i] == _moveController.GetToken(BoardTokens.TokenIsSpace))
                    {
                        _moveController.AddPiece(
                            i % countOfItemsPerSide * sideLengthPiece + margin,//XPos
                            i / countOfItemsPerSide * sideLengthPiece + margin,//YPos
                            sideLengthPiece
                            );
                        //marry to datatype in view

                        //model uses square representation with some 'positions' marked as
                        //not legal whereas the view only cares about legal positions
                        //this allows simple two way mapping 
                        _mapModelViewSpace.Add(++viewIndex, i);

                    }
                }
            }

            InitialiseGame();
        }

        private void wireUpMoveControllerHandlers()
        {
            _piecesCountHandler = (o, e) => { PiecesCount = e.PiecesCount; };
            _moveHandler = (o, e) =>
            {

                if (!_moveController.CheckForWin() && !_undoRedoManager.IsInRedo)
                {
                    //IsUndoCmdEnabled = true;
                    //_canUndo = true;


                    _undoRedoManager.AddUndo(
                        new UndoRedoState(e.UndoMove, e.MakeMove, e.SourceJumpTarget));

                }
                else
                {
                    //IsUndoCmdEnabled = false;
                    //_canUndo = false;;
                }
            };

            _hintHandler = (o, e) =>
            {
                if (ShowHintInfo) resetHintInfo();
            };


            _winHandler = (o, e) =>
            {
                IsWin = true;
                _canGetHint = false;
                resetHintInfo();
                //ShowHintInfo = false;
                GetHintCmd.CanExecute(_canGetHint);
                IsHintBtnHidden = !_isWin;
                _undoRedoManager.Reset();
                IsUndoRedoBtnHidden = !_isWin;
            };
            
            _moveController.PiecesCountChanged += _piecesCountHandler;
            _moveController.MoveChanged += _moveHandler;
            _moveController.HintChanged += _hintHandler;
            _moveController.WinChanged += _winHandler;
        }

        public void InitialiseGame()
        {
            resetWin();
            resetHintInfo();
            resetUndoRedo();

            CurrentIndex = -1; //no selection in view
            PiecesCount = 0;

            if (!_moveController.SetupStart()) return;
            if (_solver != null) _solver.LoadStart(_moveController.GetStart());
            //clear stacks

            _moveController.InitialisePiece();
            PiecesCount = _moveController.GetPiecesCount();
        }

        private void initialiseUndoRedo()
        {
            _undoRedoManager = new UndoRedoManager<UndoRedoState>();
            //EventHandler handler = (sender, eventArgs) =>
            //{
            //    _canRedo = _undoRedoManager.CanRedo;
            //    _canUndo = _undoRedoManager.CanUndo;
            //};

            resetUndoRedo();

        }

        #endregion //ctors and initialisation


        /// <summary>
        /// Check for a win - single piece remaining in the centre of the board
        /// </summary>
        private void checkForWin()
        {
            _moveController.CheckForWin(); //only fires event on win
            //if (_moveController.CheckForWin())
            //{
            //    IsWin = true;
            //    _canGetHint = false;
            //    resetHintInfo();
            //    //ShowHintInfo = false;
            //    GetHintCmd.CanExecute(_canGetHint);
            //    IsHintBtnHidden = !_isWin;
            //    _undoRedoManager.Reset();
            //    IsUndoRedoBtnHidden = !_isWin;
            //}
        }

        /// <summary>
        /// Reset Win - toggles buttons
        /// </summary>
        private void resetWin()
        {
            IsWin = false;
            _canGetHint = true;
            GetHintCmd.CanExecute(_canGetHint);
            IsHintBtnHidden = !_isWin;
            IsUndoRedoBtnHidden = !_isWin;
            resetUndoRedo();
        }

        /// <summary>
        /// Hides Hint info
        /// </summary>
        private void resetHintInfo()
        {
            ShowHintInfo = false;
            ShowHintMove(false);
        }



        /// <summary>
        /// Reset undo manager
        /// </summary>
        private void resetUndoRedo()
        {
            _undoRedoManager.Reset();
        }

        private void ShowHintMove(bool showHide)
        {
            _moveController.ShowHintMove(showHide);
        }


        #region RelayCommands

        RelayCommand _newGameCmd;

        public ICommand NewGameCmd
        {
            get
            {
                if (_newGameCmd == null)
                {
                    _newGameCmd = new RelayCommand(param => this.newGameOp(), param => true);
                }
                return _newGameCmd;
            }
        }

        void newGameOp()
        {
            Debug.WriteLine("XXXXXXXX  NEW GAME  XXXXXXXXXX");
            this.InitialiseGame();
        }

        RelayCommand _getHintCmd;

        public ICommand GetHintCmd
        {
            get
            {
                if (_getHintCmd == null)
                {
                    _getHintCmd = new RelayCommand(param => this.getHintOp(), param => _canGetHint);
                }
                return _getHintCmd;
            }
        }

        bool _canGetHint;


        /// <summary>
        /// Used by async operation to return the results of a solving search
        /// used by function invoked from getHintOp()
        /// </summary>
        struct SolveRtnState
        {
            public int IndexNextPieceToMove { get; set; }
            public int DirectionOfMove { get; set; }
            public bool IsSolution { get; set; }
        };


        /// <summary>
        /// Used to get a hint which indicates the next move to make if a solution
        /// is available or returns not available where no solution exists. Needs
        /// to solve thepuzzle in real time as it can be called for any board position
        /// at any stage int the game
        /// </summary>
        void getHintOp()
        {
            IsHintBtnHidden = false;

            if (ShowHintInfo)
            {
                resetHintInfo();
                IsHintBtnHidden = true;
                return;
            }
            _moveController.UnsetMoveStacks();

            _isSearching = true;


            //introduce a slight delay before triggering the solving animation so as to 
            //avoid flicker effect when the solving animation appears only briefly
            //on the screen where we get either no solution or a solution is returned 
            //very quickly
            new Thread(() =>
            {
                Thread.Sleep(150);
                if (_isSearching) IsSolving = true;
            }).Start();

            //provide feedback

            //a function that is to run async which attempts to solve
            //the puzzle, may be long running 
            Func<object, SolveRtnState> solve = (x) =>
            {
                _solver.LoadStart(_moveController.GetBoard());
                ulong currentBoard = _solver.CurrentBoard;
                //_solver.Solve(currentBoard);

                int indexNextPieceToMove = -1;
                int directionOfMove = -1;
                bool isSolution = false;
                _solver.Solve();
                if (_solver.IsSolution)
                {
                    indexNextPieceToMove = -1;
                    directionOfMove = -1;
                    isSolution = _solver.GetHint(ref indexNextPieceToMove, ref directionOfMove);
                }

                //report back state
                return new SolveRtnState
                {
                    IndexNextPieceToMove = indexNextPieceToMove,
                    DirectionOfMove = directionOfMove,
                    IsSolution = isSolution
                };
            };

            //solve using async
            //no object is passed in, the callback kicks off the async op and 
            //run and invokes when done rtnState
            solve.BeginInvoke(
                null,
                (IAsyncResult rtnState) =>
                {
                    //our callback that determines if we have a result and animates
                    //feed back. The solving returns are returned in rtnState which
                    //is a SolverRtnState object
                    var target = rtnState.AsyncState as Func<object, SolveRtnState>;

                    //assign our result object to what is returned from async operation
                    SolveRtnState result = target.EndInvoke(rtnState);

                    if (result.IsSolution)//is solution
                    {
                        animateHint(result.IndexNextPieceToMove, result.DirectionOfMove);
                        Debug.WriteLine("Hint index {0}, direction {1}", result.IndexNextPieceToMove, result.DirectionOfMove);
                    }
                    else
                    {
                        animateNoHint();
                    }

                    IsSolving = false;

                    _isSearching = false;



                },
                solve);
        }

        /// <summary>
        /// animate no hint
        /// </summary>
        private void animateNoHint()
        {
            ShowHintInfo = true;
            IsHintBtnHidden = true;
            HintIndexNextPieceToMove = "NA";
            HintDirectionPieceToMove = string.Empty;
            IsHintBtnHidden = true;
        }

        //animate hint
        private void animateHint(int indexNextPieceToMove, int directionOfMove)
        {
            string nswe = "áâßà";//view as wingdings font arrows
            ShowHintInfo = true;
            IsHintBtnHidden = true;
            HintIndexNextPieceToMove = indexNextPieceToMove.ToString();
            HintDirectionPieceToMove = nswe.Substring(directionOfMove, 1);
            ///TODO some fancy animation
            //unhide arrow
            //size it if necessary
            //orientate to start at index and rotate to point in direction provided
            //show it for 1 seconds

            if (!_moveController.UpdateHint(indexNextPieceToMove, directionOfMove)) return;

            ShowHintMove(true);

        }

        bool _canUndo = false;

        RelayCommand _getUndoCmd;

        public ICommand GetUndoCmd
        {
            get
            {
                if (_getUndoCmd == null)
                {
                    _getUndoCmd = new RelayCommand(param => this.getUndoOp(), param => _undoRedoManager.CanUndo);
                }
                return _getUndoCmd;
            }
        }

        private void getUndoOp()
        {
            //pop from undo and execute
            checkForWin();
            if (!IsWin && _undoRedoManager.CanUndo)
            {
                Debug.WriteLine("Invoking Undo");
                _undoRedoManager.ExecuteUndo();
            }

            IsRedoCmdEnabled = _undoRedoManager.CanRedo;
        }


        //hook up buttons on view with vm
        bool _canRedo = false;

        RelayCommand _getRedoCmd;

        public ICommand GetRedoCmd
        {
            get
            {
                if (_getRedoCmd == null)
                {
                    _getRedoCmd = new RelayCommand(param => this.getRedoOp(), param => _undoRedoManager.CanRedo);
                }
                return _getRedoCmd;
            }
        }

        private void getRedoOp()
        {
            //pop from redo and execute
            if (!IsWin && _undoRedoManager.CanRedo)
            {
                Debug.WriteLine("Invoking Redo");
                _undoRedoManager.ExecuteRedo();
            }

            IsUndoCmdEnabled = _undoRedoManager.CanUndo;
        }

        #endregion //end relay commands


        protected override void OnDispose()
        {
            _moveController.PiecesCountChanged -= _piecesCountHandler;
            _moveController.MoveChanged -= _moveHandler;
            _moveController.HintChanged -= _hintHandler;
            _moveController.WinChanged -= _winHandler;
            
            base.OnDispose();
        }
    }
}
