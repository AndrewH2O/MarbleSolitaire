using CrossCuttingLib.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireModelLib.Model
{
    public class Game
    {
        public readonly int TokenHasPiece;
        public readonly int TokenIsSpace;
        public readonly int TokenUnknown;

        List<int> _boardItems;

        public List<int> BoardItems
        {
            get { return _boardItems; }
            private set { _boardItems = value; }
        }
        
        SquareBoard _squareBoard;

        public SquareBoard SquareBoard
        {
            get { return _squareBoard; }
            private set { _squareBoard = value; }
        }
        
        /// <summary>
        /// Accepts a board that is pre loded with a start position
        /// </summary>
        /// <param name="board"></param>
        public Game(Board board)
        {

            if (board == null || board.HasStart == false) throw new Exception("board setup error detected");

            _squareBoard = (SquareBoard)board;

            TokenHasPiece = _squareBoard.TokenHasPiece;
            TokenIsSpace = _squareBoard.TokenIsSpace;
            TokenUnknown = _squareBoard.TokenUnknown;
        }

        public IEnumerable<int> GetBoardContents()
        {
            foreach (var item in _squareBoard.BoardItems)
            {
                yield return item;
            }
        }

        
    }
}
