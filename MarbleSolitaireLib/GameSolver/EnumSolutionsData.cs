using MarbleSolitaireLib.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{
    public class EnumSolutionsData
    {
        const int NUMBER_ROTATEFLIPS = 8;
        const int MAX_NUMBER_PIECES = 32;
        
        RotateFlip _rf;
        EnumSolutionsDTO _dto;

        
        //List<ulong>[] _listBoards;
        
        //public ulong this[ulong piecesCount, int indexInListBoardsByPiece]
        //{
            
        //    //get { return _listBoards[piecesCount][boardIndex]; }
        //}

        
        //public List<ulong> this[ulong piecesCount]
        public int this[ulong piecesCount]
        {
            get { return _countOfUniqueBoardsByPieces[piecesCount]; }
            //get { return _listBoards[piecesCount]; }
        }
        
        ulong[] _rotatedFlippedBoards = new ulong[NUMBER_ROTATEFLIPS];

        int[] _countOfUniqueBoardsByPieces = new int[MAX_NUMBER_PIECES];
        int _sumOfcountOfUniqueBoardsByPieces = 0;
        int[] _countOfDistinctBoardsByPieces = new int[MAX_NUMBER_PIECES];
        int _sumOfCountOfDistinctBoardsByPieces = 0;
        public EnumSolutionsData(EnumSolutionsDTO dto, RotateFlip rf)
        {
            _dto = dto;
            _rf = rf;
            GetCountOfUniqueSolutions();
        }


        //private void initialise()
        //{
        //    _listBoards = new List<ulong>[MAX_NUMBER_PIECES];
            
        //    for (int i = 0; i < MAX_NUMBER_PIECES; i++)
        //    {
        //        _listBoards[i] = new List<ulong>();

        //    }
        //}

        private int getPiecesCount(ulong board)
        {
            int count=0;
            for (int i = 0; i < MAX_NUMBER_PIECES+1; i++)
            {
                if((board & (1ul << i)) != 0)count++;
            }
            return count;
        }

        public bool IsSolution(ulong board, ulong piecesCount)
        {
            _rf.GetRotationsFlipsForBoard(board, _rotatedFlippedBoards);

            ulong resultBoard = default(ulong);
            for (int i = 0; i < NUMBER_ROTATEFLIPS; i++)
            {
                resultBoard= Array.Find(_dto[piecesCount - 1], x => x == _rotatedFlippedBoards[i]);
                if(resultBoard!=default(ulong))return true;
            }
            
            return false;
        }


        public bool IsSolution(ulong board)
        {
            return IsSolution(board, (ulong)getPiecesCount(board));
        }


        public int GetCountOfUniqueSolutions()
        {
            int count = 0;
            for (ulong i = 0; i < MAX_NUMBER_PIECES; i++)
            {
                _countOfUniqueBoardsByPieces[i] = _dto[i].GetLength(0);
                count += _countOfUniqueBoardsByPieces[i];
            }
            
            _sumOfcountOfUniqueBoardsByPieces = count;
            return count;
        }

        public int GetCountTotalDistinctSolutions()
        {
            int count = 0;

            ulong[] rf = new ulong[NUMBER_ROTATEFLIPS];
            
            
            for (ulong i = 0; i < MAX_NUMBER_PIECES; i++)
            {
                List<ulong> boardsPerPieceCount = new List<ulong>();
                Array.Clear(rf, 0, NUMBER_ROTATEFLIPS);
                foreach (ulong item in _dto[i])
                { 
                    _rf.GetRotationsFlipsForBoard(item, rf);
                    boardsPerPieceCount.AddRange(rf.Distinct());
                }

                _countOfDistinctBoardsByPieces[i]=boardsPerPieceCount.Count();
                count += _countOfDistinctBoardsByPieces[i];
            }
            
            _sumOfCountOfDistinctBoardsByPieces = count;
            return count;
        }


        /// <summary>
        /// Displays by piece count the total unique number of winning boards
        /// this is after rotations and symmetry
        /// </summary>
        /// <returns>result as formatted string</returns>
        public string DisplayUniqueBoardsCountsByPiece()
        {
            if(_sumOfcountOfUniqueBoardsByPieces==0) GetCountOfUniqueSolutions();
            StringBuilder sb = new StringBuilder();
            int numberOfBoards;
            sb.AppendLine("Unique number of winning boards listed by pieces count");
            sb.AppendLine("Pieces Count, Number of boards");
            
            for (ulong i = 0; i < MAX_NUMBER_PIECES; i++)
            {
                numberOfBoards = _countOfUniqueBoardsByPieces[i];
                sb.Append(string.Format("{0,3:D0} :{1,10:N0}", i + 1, numberOfBoards));
                sb.AppendLine();
            }
            sb.AppendLine("      ---------");
            sb.AppendLine(string.Format("{0,15:N0}", _sumOfcountOfUniqueBoardsByPieces));
            return sb.ToString();
        }

        /// <summary>
        /// Displays by piece count the total number of distinct winning boards
        /// includes rotations and symmetry
        /// </summary>
        /// <returns>result as formatted string</returns>
        public string DisplayTotalDistinctBoardsCountsByPiece()
        {
            if(_sumOfCountOfDistinctBoardsByPieces==0)GetCountTotalDistinctSolutions();
            StringBuilder sb = new StringBuilder();
            int numberOfBoards;
            sb.AppendLine("Total distinct number of winning boards listed by pieces count");
            sb.AppendLine("Pieces Count, Number of boards");
            
            for (ulong i = 0; i < MAX_NUMBER_PIECES; i++)
            {
                numberOfBoards = _countOfDistinctBoardsByPieces[i];
                sb.Append(string.Format("{0,3:D0} :{1,12:N0}", i + 1, numberOfBoards));
                sb.AppendLine();
            }
            sb.AppendLine("       ----------");
            sb.AppendLine(string.Format("{0,17:N0}", _sumOfCountOfDistinctBoardsByPieces));
            return sb.ToString();
        }
    }
}
