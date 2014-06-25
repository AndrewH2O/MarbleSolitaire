using MarbleSolitaireModelLib.Model;
using MarbleSolitaireViewModel.Moves;
using MarbleSolitaireViewModel.ViewHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireViewModel.ViewModel.Moves
{

    

    

    /// <summary>
    /// Responsible for moves
    /// </summary>
    public class ViewMoveController : MarbleSolitaireViewModel.Moves.IViewMoveController
    {
        readonly int ERROR = -1;

        
        /// <summary>
        /// notify piecesCount change
        /// </summary>
        public event PiecesCountHandler PiecesCountChanged;

        protected virtual void OnPiecesCountChanged(PiecesCountArgs e)
        {
            if (PiecesCountChanged != null)
                PiecesCountChanged(this, e);
        }

        void updatePieces(int piecesCount)
        {
            OnPiecesCountChanged(new PiecesCountArgs() { PiecesCount = piecesCount });
        }

        /// <summary>
        /// notify move change
        /// </summary>
        public event MoveHandler MoveChanged;

        protected virtual void OnMoveChanged(MoveArgs e)
        {
            if (MoveChanged != null)
                MoveChanged(this, e);
        }


        /// <summary>
        /// notify hint change
        /// </summary>
        public event HintHandler HintChanged;

        protected virtual void OnHintChanged(HintArgs e)
        {
            if (HintChanged != null)
                HintChanged(this, e);
        }

        void updateHint(HintInfoState hintInfoState)
        {
            switch (hintInfoState)
            {
                case HintInfoState.Reset:
                    OnHintChanged(new HintArgs() { HintInfoState = HintInfoState.Reset });
                    break;
                case HintInfoState.Show:
                    OnHintChanged(new HintArgs() { HintInfoState = HintInfoState.Show });
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Notify of a Win
        /// </summary>
        public event WinHandler WinChanged;

        protected virtual void OnWinChanged(EventArgs e)
        {
            if (WinChanged != null)
                WinChanged(this, e);
        }

        void updateWin()
        {
            OnWinChanged(new EventArgs());
        }



        int COUNT_PIECES_FOR_WIN = 1;
        int HAS_PIECE = 1;
        int CENTRE_INDEX = 16;

        int _piecesCount;
        
        /// <summary>
        /// Maps model and view coordinates
        /// </summary>
        IMapModelViewSpace _mapModelViewSpace;

        /// <summary>
        /// Layout type defines a model that is easy to index and specifies if a location
        /// is a legal board position and the state of that position
        /// </summary>
        ISquareBoard _sqb;

        /// <summary>
        /// For each piece (ie cell) show available moves
        /// </summary>
        Stack<SourceJumpTarget> _possibleMovesStack = new Stack<SourceJumpTarget>();

        /// <summary>
        /// Selected piece stack
        /// </summary>
        Stack<int> _selectedPiecesStack = new Stack<int>();

        /// <summary>
        /// piece locate in a cell on the board - the state of a piece at a location
        /// changes not the position of a piece.
        /// </summary>
        ObservableCollection<Piece> _pieces = new ObservableCollection<Piece>();




        public ObservableCollection<Piece> Pieces
        {
            get { return _pieces; }
        }

        public ViewMoveController(ISquareBoard board, IMapModelViewSpace mapModelViewSpace)
        {
            _sqb = board;
            _mapModelViewSpace = mapModelViewSpace;
        }

        public List<int> GetBoard()
        {
            return _sqb.BoardItems;
        }

        public int GetSide()
        {
            return _sqb.Side;
        }



        public int GetToken(BoardTokens token)
        {
            switch (token)
            {
                case BoardTokens.TokenHasPiece:
                    return _sqb.TokenHasPiece;

                case BoardTokens.TokenIllegalPosition:
                    return _sqb.TokenIllegalPosition;

                case BoardTokens.TokenIsSpace:
                    return _sqb.TokenIsSpace;

                case BoardTokens.TokenUnknown:
                    return _sqb.TokenUnknown;

                default:

                    break;
            }

            return _sqb.TokenUnknown;
        }

        public bool SetupStart()
        {
            if (_sqb.Start == null) return false;
            _sqb.SetupStart(_sqb.Start);
            return true;
        }

        public List<int> GetStart()
        {
            return _sqb.Start;
        }

        public int GetPiecesCount()
        {
            return _piecesCount;
        }


        public void InitialisePiece()
        {
            clearStacks();

            int viewIndex = 0;

            List<int> simpleBoardInModelCoords = _sqb.BoardItems;

            if (simpleBoardInModelCoords != null && simpleBoardInModelCoords.Count > 0)
            {
                for (int i = 0; i < simpleBoardInModelCoords.Count; i++)
                {
                    if (!(viewIndex < _mapModelViewSpace.Count())) break;
                    ///TODO add auto resize behaviour
                    if (_mapModelViewSpace.ConvertToModelIndex(viewIndex) == i)
                    {

                        if (simpleBoardInModelCoords[i] == _sqb.TokenHasPiece || simpleBoardInModelCoords[i] == _sqb.TokenIsSpace)
                        {
                            InitialisePiece(viewIndex, simpleBoardInModelCoords[i], true);
                            if (simpleBoardInModelCoords[i] == _sqb.TokenHasPiece) ++_piecesCount;
                        }
                        else
                        {
                            InitialisePiece(viewIndex, _sqb.TokenUnknown, false);
                        }

                        viewIndex++;
                    }
                }
            }

            updatePieces(_piecesCount);

        }


        public bool CheckForWin()
        {
            bool isWin = _piecesCount == COUNT_PIECES_FOR_WIN && _pieces[CENTRE_INDEX].Content == HAS_PIECE;
            if (isWin) updateWin();
            return isWin;
        }


        /// <summary>
        /// Update selected, evaluate possible moves and if we are 
        /// selecting a target for a jump pmake move 
        /// </summary>
        /// <param name="currentIndex">Current Index</param>
        public void UpdateSelectedPiece(int currentIndex)
        {

            if (!ValidateCurrentIndex(currentIndex)) return;


            //if we can move do so and return
            if (canMove(currentIndex))
            {
                //making a move hide hint
                updateHint(HintInfoState.Reset);
                
                return;
            }

            UnsetMoveStacks();

            //set selected
            _pieces[currentIndex].IsSelected = true;

            _selectedPiecesStack.Push(currentIndex);

            //get jumps and targets
            int[] jumpees = new int[4];
            jumpees = _sqb.GetListOfJumpedCandidates(_mapModelViewSpace.ConvertToModelIndex(currentIndex));
            int[] targets = new int[4];
            targets = _sqb.GetListOfTargetCandidates(_mapModelViewSpace.ConvertToModelIndex(currentIndex));

            //moves?
            evaluatePossibleMoves(currentIndex, jumpees, targets);
        }


        public void ShowHintMove(bool showHide)
        {
            if (_hintMoveSourceJumpTarget == null) return;
            _pieces[_hintMoveSourceJumpTarget.Source].IsSelected = showHide;
            _pieces[_hintMoveSourceJumpTarget.Source].IsSourceCandidate = showHide;
            _pieces[_hintMoveSourceJumpTarget.Jump].IsJumpCandidate = showHide;
            _pieces[_hintMoveSourceJumpTarget.Target].IsTargetCandidate = showHide;
        }




        public bool UpdateHint(int indexNextPieceToMove, int directionOfMove)
        {
            if (!ValidateCurrentIndex(indexNextPieceToMove)) return false;
            else
            {
                //get jumps and targets
                int[] jumpees = new int[4];
                jumpees = _sqb.GetListOfJumpedCandidates(_mapModelViewSpace.ConvertToModelIndex(indexNextPieceToMove));
                int[] targets = new int[4];
                targets = _sqb.GetListOfTargetCandidates(_mapModelViewSpace.ConvertToModelIndex(indexNextPieceToMove));

                if (!(directionOfMove >= 0 && directionOfMove < jumpees.Length)) return false;///TODO error

                _hintMoveSourceJumpTarget = new SourceJumpTarget()
                {
                    Source = indexNextPieceToMove,
                    Jump = _mapModelViewSpace.ConvertToViewIndex(jumpees[directionOfMove]),
                    Target = _mapModelViewSpace.ConvertToViewIndex(targets[directionOfMove])
                };

                return true;
            }


        }


        /// <summary>
        /// Clear move stacks
        /// </summary>
        public void UnsetMoveStacks()
        {
            //reset 
            if (_selectedPiecesStack.Count > 0)
            {
                _pieces[_selectedPiecesStack.Pop()].IsSelected = false;
            }
            //unset source
            while (_sourcePiecesStack.Count > 0)
            {
                _pieces[_sourcePiecesStack.Pop()].IsSourceCandidate = false;
            }

            //unset jumps
            while (_jumpedPiecesStack.Count > 0)
            {
                _pieces[_jumpedPiecesStack.Pop()].IsJumpCandidate = false;
            }

            //unset targets
            while (_targetPiecesStack.Count > 0)
            {
                _pieces[_targetPiecesStack.Pop()].IsTargetCandidate = false;
            }
        }

        /// <summary>
        /// From the possible moves check which one we are making and if we
        /// can make it then do so.
        /// </summary>
        /// <param name="currentIndex">Current Index</param>
        /// <returns>true if moved elese false</returns>
        private bool canMove(int currentIndex)
        {
            SourceJumpTarget move;
            bool isMoveMade = false;
            while (_possibleMovesStack.Count > 0)
            {
                move = _possibleMovesStack.Pop();
                if (_mapModelViewSpace.ConvertToViewIndex(move.Target) == currentIndex)
                {
                    Debug.WriteLine("vm-canMake->makeMove sjt {0} {1} {2}", move.Source, move.Jump, move.Target);
                    makeMove(move);
                    isMoveMade = true;
                    CheckForWin();
                    break;
                }
            }
            _possibleMovesStack.Clear();
            return (isMoveMade ? true : false);
        }



        /// <summary>
        /// Store source viewIndex, jump as model Index, target as model index
        /// </summary>
        /// <param name="move">move as SourceJumpTarget format</param>
        private void makeMove(SourceJumpTarget move)
        {
            Debug.WriteLine("vm-makeMove sjt {0} {1} {2}", move.Source, move.Jump, move.Target);
            //if (ShowHintInfo)
            //{
            //    //ShowHintInfo = false;
            //    updateHint(HintInfoState.Reset);
            //}
            updateHint(HintInfoState.Reset); //raise request regardless and let view decide if ShowHintInfo then reset

            //**updateModel*****
            _sqb.MakeMove(_mapModelViewSpace.ConvertToModelIndex(move.Source), move.Jump, move.Target);

            //**updateView******

            //we no longer wish to show possible moves as we have picked one
            UnsetMoveStacks();

            //make move
            _pieces[move.Source].Content = 0;
            _pieces[_mapModelViewSpace.ConvertToViewIndex(move.Jump)].Content = 0;
            _pieces[_mapModelViewSpace.ConvertToViewIndex(move.Target)].Content = 1;

            //update count of pieces
            updatePieces(--_piecesCount);
            OnPiecesCountChanged(new PiecesCountArgs() { PiecesCount = _piecesCount });

            //track piece index update moved piece and reset removed ones
            /*
            _pieces[convertToViewIndexFromModel(move.Target)].Index=_pieces[move.Source].Index;
            _pieces[move.Source].Index = -1;
            _pieces[convertToViewIndexFromModel(move.Jump)].Index = -1;
             */

            //reset selected
            _pieces[_mapModelViewSpace.ConvertToViewIndex(move.Target)].IsSelected = false;

            CheckForWin();

            SourceJumpTarget sjt = move.Copy();
            OnMoveChanged(new MoveArgs
            {
                MakeMove = x => this.makeMove(sjt),
                UndoMove = x => this.undoMove(sjt),
                SourceJumpTarget = sjt
            });


        }

        /// <summary>
        /// Validate index
        /// </summary>
        /// <param name="currentIndex"current index></param>
        /// <returns>is valid state</returns>
        public bool ValidateCurrentIndex(int currentIndex)
        {
            if (!(currentIndex != -1 && currentIndex >= 0 && currentIndex < _pieces.Count))
                return false;
            else
                return true;
        }

        /// <summary>
        /// store source viewIndex, jump as model Index target as model index
        /// </summary>
        /// <param name="move">move as SourceJumpTarget format</param>
        private void undoMove(SourceJumpTarget move)
        {
            Debug.WriteLine("vm-undoMove sjt {0} {1} {2}", move.Source, move.Jump, move.Target);
            
            //if (ShowHintInfo)
            //{
            //    //ShowHintInfo = false;
            //    updateHint(HintInfoState.Reset);
            //}
            updateHint(HintInfoState.Reset); //raise request regardless and let view decide if ShowHintInfo then reset

            //**updateModel*****
            _sqb.UnMakeMove(_mapModelViewSpace.ConvertToModelIndex(move.Source), move.Jump, move.Target);

            //**updateView******

            //we no longer wish to show possible moves as we have picked one
            UnsetMoveStacks();

            //make move
            _pieces[move.Source].Content = 1;
            _pieces[_mapModelViewSpace.ConvertToViewIndex(move.Jump)].Content = 1;
            _pieces[_mapModelViewSpace.ConvertToViewIndex(move.Target)].Content = 0;

            //update count of pieces
            updatePieces(++_piecesCount);



            //track piece index update moved piece and reset removed ones
            /*
            _pieces[convertToViewIndexFromModel(move.Target)].Index=_pieces[move.Source].Index;
            _pieces[move.Source].Index = -1;
            _pieces[convertToViewIndexFromModel(move.Jump)].Index = -1;
             */

            //reset selected
            _pieces[_mapModelViewSpace.ConvertToViewIndex(move.Target)].IsSelected = false;
        }


        SourceJumpTarget _hintMoveSourceJumpTarget;





        /// <summary>
        /// Store possible moves by evaluating alternatives on the model and storing all
        /// that are legal by comparing with the current board position. 
        /// </summary>
        /// <param name="currentIndex">current view Index</param>
        /// <param name="jumpees">possible jump candidates</param>
        /// <param name="targets">possible targets</param>
        private void evaluatePossibleMoves(int currentIndex, int[] jumpees, int[] targets)
        {
            if (!(jumpees.Length > 0 && targets.Length > 0 && targets.Length == jumpees.Length)) return;
            for (int i = 0; i < jumpees.Length; i++)
            {
                if (_sqb.CheckMove(
                    _mapModelViewSpace.ConvertToModelIndex(currentIndex), jumpees[i], targets[i]))
                {
                    _possibleMovesStack.Push(
                        new SourceJumpTarget()
                        {
                            Source = currentIndex,
                            Jump = jumpees[i],
                            Target = targets[i]
                        });
                    updateSourceCandidates(currentIndex);
                    updateJumpCandidates(jumpees[i]);
                    updateTargetCandidates(targets[i]);
                }
            }
        }

        //update source jump and targets

        Stack<int> _sourcePiecesStack = new Stack<int>();
        private void updateSourceCandidates(int indexView)
        {
            //set
            _pieces[indexView].IsSourceCandidate = true;
            _sourcePiecesStack.Push(indexView);
        }


        Stack<int> _jumpedPiecesStack = new Stack<int>();
        private void updateJumpCandidates(int indexModel)
        {
            //set
            int indexView = _mapModelViewSpace.ConvertToViewIndex(indexModel);
            if (indexView != ERROR)
            {
                _pieces[indexView].IsJumpCandidate = true;
                _jumpedPiecesStack.Push(indexView);
            }

        }

        Stack<int> _targetPiecesStack = new Stack<int>();
        private void updateTargetCandidates(int indexModel)
        {
            int indexView = _mapModelViewSpace.ConvertToViewIndex(indexModel);
            if (indexView != ERROR)
            {
                _pieces[indexView].IsTargetCandidate = true;
                _targetPiecesStack.Push(indexView);
            }
        }



        void clearStacks()
        {
            _sourcePiecesStack.Clear();
            _jumpedPiecesStack.Clear();
            _targetPiecesStack.Clear();
            _possibleMovesStack.Clear();
            _selectedPiecesStack.Clear();
        }

        public void AddPiece(int XPos, int YPos, int SideLengthPiece)
        {
            _pieces.Add(new Piece()
            {
                //Content=simpleBoardInModelCoords[i], 

                XPos = XPos,
                YPos = YPos,
                SideLengthPiece = SideLengthPiece
            }
                       );
        }

        public void InitialisePiece(int Index, int Content, bool IsValidGamePos)
        {
            _pieces[Index].Content = Content;
            _pieces[Index].Index = Index;
            if (IsValidGamePos)
            {
                _pieces[Index].IsSelected = false;
                _pieces[Index].IsJumpCandidate = false;
                _pieces[Index].IsSourceCandidate = false;
                _pieces[Index].IsTargetCandidate = false;
            }
        }

        

    }
}
