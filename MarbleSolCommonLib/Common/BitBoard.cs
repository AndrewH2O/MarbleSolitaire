using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolCommonLib.Common
{
    /// <summary>
    /// Represents a 2d board of equal sides as a single UInt64
    /// Provides basic functionality to manage legal from non legal moves on a bit board
    /// and basic bit manipulation
    /// </summary>
    public class BitBoard
    {
        protected const  int LEGAL_POSITION = 1;
        protected const int MAX_SIZE = 63;
        protected UInt64 _boardLegal = 0;
        protected UInt64 _boardIllegal = 0;
        protected UInt64 _boardCurrent = 0;
        private UInt64 _boardWin = 0;

        

        protected int _maxValueLegalPositions;//# legal positions

        public UInt64 BoardCurrent
        {
            get { return _boardCurrent; }
            private set { _boardCurrent = value; }
        }

        public UInt64 BoardWin
        {
            get { return _boardWin; }
            private set { _boardWin = value; }
        }
        
        /// <summary>
        /// Constructor accepts a list a marking the legal positions on the board,
        /// should be 0 if not legal and 1 if legal. 
        /// </summary>
        /// <param name="legalPositions"></param>
        public BitBoard(List<int> legalPositions)
        {
            setLegal_Illegal(legalPositions);
        }

        /// <summary>
        /// Is the move legal
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsIllegal(int index)
        {
            return testBit(index, _boardIllegal);
        }


        public bool IsSet(int index)
        {
            return testBit(index, _boardCurrent);
        }

        public bool IsSet(int index, ulong value)
        {
            return testBit(index, value);
        }

        public void SetCurrentPositions(List<int> currentPositions)
        {
            _boardCurrent = 0;
            setPositions(currentPositions,ref _boardCurrent);
            //Debug.WriteLine(DisplayBoard(_boardCurrent,"_boardCurrent",7));
        }

        public void SetWinPosition(List<int> winPositions)
        {
            _boardWin = 0;
            setPositions(winPositions, ref _boardWin);
            //Debug.WriteLine(DisplayBoard(_boardCurrent,"_boardCurrent",7));
        }

        void setPositions(List<int> currentPositions, ref ulong board)
        {
            if (!(currentPositions != null || currentPositions.Count > 0)) return;
            int count = currentPositions.Count;
            if (!validate(count - 1)) return;
            
            for (int i = 0; i < count; i++)
            {
                if (currentPositions[i] == 1)
                {
                    setBit(i, ref board);
                }
            }
        }

        public void ClearBoard()
        {
            _boardCurrent = 0;
        }

        /// <summary>
        /// Count pieces on board
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public int CountPieces(ulong board)
        {
            int count = 0;

            for (int i = 0; i < _maxValueLegalPositions; i++)
            {
                if (this.testBit(i, board)) count++;
            }
            return count;
        }

        public string DisplayBoard(ulong _boardCurrent, string description, int side)
        {
            int length = side * side;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append((testBit(i, _boardCurrent) ? "1" : "0"));
                if (i > 0 && (i + 1) % side == 0) sb.Append("\n");
            }
            return "<===" + description + "===>\n" + sb.ToString() + "\n<============+============>";
        }

        /// <summary>
        /// stores the legal and illegal positions of a 2d board
        /// as two uint64s respectively
        /// </summary>
        /// <param name="legalPositions"></param>
        protected void setLegal_Illegal(List<int> legalPositions)
        {
            _boardCurrent = 0;
            if (!(legalPositions != null || legalPositions.Count != 0)) return;
            _maxValueLegalPositions = legalPositions.Count;
            for (int i = 0; i < _maxValueLegalPositions; i++)
            {
                if (legalPositions[i] == LEGAL_POSITION)
                {
                    setBit(i, ref _boardLegal);
                }
                else
                {
                    setBit(i, ref _boardIllegal);
                }
            }
        }


        public bool testBit(int index, UInt64 board)
        {
            if (!validate(index)) return true;
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

        protected bool validate(int index)
        {
            return (index >= 0 && index < MAX_SIZE) ? true : false;
            //if (index > MAX_SIZE) return false;
            //return ((index >= 0 && index < _maxValueLegalPositions) ? true : false);
        }
    }
}
