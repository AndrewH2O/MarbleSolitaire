

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarbleSolCommonLib.Common;
using CrossCuttingLib.Errors;
using System.Diagnostics;


namespace MarbleSolitaireModelLib.Model
{
    public class SquareBoard:Board,ICandidates, IBoard, MarbleSolitaireModelLib.Model.ISquareBoard
    {
        List<Node4> _nodes;//7*7
        

        
        IErrorLog _errorLog;


        protected int _side;

        public int Side
        {
            get { return _side; }
            protected set { _side = value; }
        }

        

        int _countOfDimensions;
        public int CountOfDimensions
        {
            get { return _countOfDimensions; }
        }

        
        
        SquareDTO _dto;
        
        private IErrorLog errorLog;
        
        public SquareBoard(List<int> legalPositions, IErrorLog errorLog ):base(legalPositions)
        {
            // TODO: Complete member initialization
            if(_errorLog!=null)_errorLog = errorLog;
            BitBoard bitBoard = new BitBoard(legalPositions);
            initialise(bitBoard);
        }

       

        private void initialise(BitBoard bitBoard)
        {
            _countOfDimensions = Node4.NUMBER_LINK;
            _side = (int)Math.Sqrt(_boardItems.Count);
            if(_side*_side!=this._length) _errorLog.Message("SquareBoard","01000");
            
            //calculate lookup link rules
            _dto = new SquareDTO(bitBoard, _side);
            _nodes = new List<Node4>(_boardItems.Count);
            
            //add board contents based on content is 1 where there is a legal position
            for (int i = 0; i < _boardItems.Count; i++)
            {
                _nodes.Add(new Node4());
                if (_boardItems[i] == 1) _nodes[i].Content = 1;
            }

            //cache lookup links in nodes using DTO to process rules and map data
            int index = 0;
            for (int i = 0; i < _side; i++)
            {
                for (int j = 0; j < _side; j++)
                {
                    _dto.UpdateLxInNode(_nodes[index++], i, j);
                }
            }
        }

        


        /// <summary>
        /// Setup the game start position
        /// expected that value in start board:
        /// 1 for a piece,
        /// 0 for a space,
        /// -1 for illegal position
        /// anything else will be treated as unknown
        /// </summary>
        /// <param name="start"></param>
        public override void SetupStart(List<int> start)//bool hasStart = true
        {
            base.validateStart(start);
            
            _countPieces = 0;
            int token;
            ClearBoard();
            Start = start;
            for (int i = 0; i < start.Count; i++)
            {
                if (_dto.isValidGameIndex(i))
                {
                    token = start[i];
                    if (token == this.TokenHasPiece || token == this.TokenIsSpace)
                    {
                        _boardItems[i] = start[i];
                        if (token == this.TokenHasPiece) _countPieces++;
                    }
                    else
                    {
                        _boardItems[i] = this.TokenUnknown;
                    }
                }
                else
                {
                    _boardItems[i] = this.TokenIllegalPosition;
                }
            }

            _countPiecesStart = _countPieces;
        }


        /// <summary>
        /// Checks if a move is valid by checking legal moves for a piece that is going to be moved
        /// and if the corresponding pieces and spaces are in the correct sequence
        /// </summary>
        /// <param name="start">start pieces index </param>
        /// <param name="jumped">jumped pieces index</param>
        /// <param name="target">target space index</param>
        /// <returns></returns>
        public override bool CheckMove(int start, int jumped, int target)
        {
            if (!_dto.isValidGameIndex(start)) return false; //_errorLog.Message("SquareBoard", "02002");
            if (!_dto.isValidGameIndex(jumped)) return false; //_errorLog.Message("SquareBoard", "02003");
            if (!_dto.isValidGameIndex(target)) return false; // _errorLog.Message("SquareBoard", "02004");
            
            if (IsMoveLegal(_nodes[start],start, jumped, target))
            {
                //are pieces and spaces correct
                if (_boardItems[start] == 1 && _boardItems[jumped] == 1 && _boardItems[target] == 0) return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a move is valid by checking legal moves for a piece that is going to be moved
        /// and if the corresponding pieces and spaces are in the correct sequence
        /// </summary>
        /// <param name="start">start pieces index </param>
        /// <param name="jumped">jumped pieces index</param>
        /// <param name="target">target space index</param>
        /// <returns></returns>
        public override bool CheckUndoMove(int start, int jumped, int target)
        {
            if (!_dto.isValidGameIndex(start)) return false; //_errorLog.Message("SquareBoard", "02002");
            if (!_dto.isValidGameIndex(jumped)) return false; //_errorLog.Message("SquareBoard", "02003");
            if (!_dto.isValidGameIndex(target)) return false; // _errorLog.Message("SquareBoard", "02004");

            if (IsMoveLegal(_nodes[start], start, jumped, target))
            {
                //are pieces and spaces correct
                if (_boardItems[start] == 0 && _boardItems[jumped] == 0 && _boardItems[target] == 1) return true;
            }

            return false;
        }


        /// <summary>
        /// Checks if the moves are valid and accepts start, jumped and 
        /// target indexes. These indexes are based on a single dimensional array
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="jumped">jumped index</param>
        /// <param name="target">target index</param>
        /// <returns>is move valid?</returns>
        private bool IsMoveLegal(Node4 node, int start, int jumped, int target)
        {
            if (start == node.Source[0])//one each for NSWE index but they are all the same
            {
                int currentDirectionIndex = -1;
                //check if jumped is valid for this node
                for (int i = 0; i < node.Jumped.Length; i++)
                {
                    if (jumped == node.Jumped[i])
                    {
                        currentDirectionIndex = i;//nswe
                        break;
                    }
                }
                
                //check if we found jumped index associated with our source
                //current index position indicate direction if it were valid
                if (currentDirectionIndex == -1) return false;

                //check if target is OK
                if (!(node.Targets[currentDirectionIndex] == target)) return false;
            }
            else
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// If move is vallid updates board
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="jumped">jumped index</param>
        /// <param name="target">target index</param>
        public override void MakeMove(int start, int jumped, int target)
        {
            Debug.WriteLine("sqb-makeMove sjt {0} {1} {2}", start, jumped, target);
            if(CheckMove(start,jumped,target))
            {
                Debug.WriteLine("sqb-checkMove-ok");
                _boardItems[start] = 0;
                _boardItems[jumped] = 0;
                _boardItems[target] = 1;
                _countPieces--;
            }

            CheckForWin();
        }

        /// <summary>
        /// If move is vallid updates board
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="jumped">jumped index</param>
        /// <param name="target">target index</param>
        public override void UnMakeMove(int start, int jumped, int target)
        {
            Debug.WriteLine("sqb-undoMove sjt {0} {1} {2}", start, jumped, target);
            if(CheckForStart())return;
            
            if (CheckUndoMove(start, jumped, target))
            {
                Debug.WriteLine("sqb-checkMove-ok");
                _boardItems[start] = 1;
                _boardItems[jumped] = 1;
                _boardItems[target] = 0;
                _countPieces++;
            }

            CheckForStart();
        }


        public bool CheckForWin()
        {
            if (_countPieces == 1 && _boardItems[24] == 1)
            { 
                return true; 
            }
            else
            {
                return false;
            }
        }

        public bool CheckForStart()
        {
            if (_countPieces == _countPiecesStart)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int[] GetListOfJumpedCandidates(int index)
        {
            if (!validateIndex(index)) return null;
            return _nodes[index].Jumped;
        }

        public override int[] GetListOfTargetCandidates(int index)
        {
            if (!validateIndex(index)) return null;
            return _nodes[index].Targets;
        }

        public override int[] GetListOfSourceCandidates(int index)
        {
            if (!validateIndex(index)) return null;
            return _nodes[index].Source;
        }

        private bool validateIndex(int index)
        {
            return (index > 0 && index < _nodes.Count() && _dto.isValidGameIndex(index));
        }

        public IEnumerable<int> EnumerateNodesByIndex(Predicate<Node4> filter)
        {
            foreach (var item in _nodes)
            {
                if (filter(item)) yield return item.Source[0];
            }
        }
    }

    
}
