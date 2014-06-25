

using MarbleSolCommonLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MarbleSolitaireLib.GameSolver
{
    /// <summary>
    /// Board provides functionality to work with moves represented by 
    /// pre, post and mask bits
    /// </summary>
    public class SolverBoard:BitBoard
    {
        //http://community.topcoder.com/tc?module=Static&d1=tutorials&d2=bitManipulation

        
        public SolverBoard(List<int> legalPositions):base(legalPositions){}
        

        /// <summary>
        /// Post move position so source 0, jumped 0, target 1
        /// </summary>
        /// <param name="source"></param>
        /// <param name="jumped"></param>
        /// <param name="target"></param>
        /// <param name="move">PostMove as a ref ulong</param>
        /// <returns>returns false if source, jumped or target are invalid indexes</returns>
        public bool SetPostMove(int source, int jumped, int target, ref ulong move)
        {
            if (validate(source) && validate(jumped) && validate(target))
            {
                clearBit(source, ref move);
                clearBit(jumped, ref move);
                setBit(target, ref move);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Pre move position so source = 1, jumped=1 and target=0
        /// </summary>
        /// <param name="source"></param>
        /// <param name="jumped"></param>
        /// <param name="target"></param>
        /// <param name="move">PreMove as a ref ulong</param>
        /// <returns>returns false if source, jumped or target are invalid indexes</returns>
        public bool SetPreMove(int source, int jumped, int target, ref ulong move)
        {
            if (validate(source) && validate(jumped) && validate(target))
            {
                setBit(source, ref move);
                setBit(jumped, ref move);
                clearBit(target, ref move);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Mask to obtain significant bits source = 1, jumped=1 and target=1
        /// </summary>
        /// <param name="source"></param>
        /// <param name="jumped"></param>
        /// <param name="target"></param>
        /// <param name="move">Mask as a ref ulong</param>
        /// <returns>returns false if source, jumped or target are invalid indexes</returns>
        public bool SetMaskMove(int source, int jumped, int target, ref ulong move)
        {
            if (validate(source) && validate(jumped) && validate(target))
            {
                setBit(source, ref move);
                setBit(jumped, ref move);
                setBit(target, ref move);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks that a move is possible so the indexes must be valid as must the pre-conditions
        /// for the move
        /// </summary>
        /// <param name="source"></param>
        /// <param name="jumped"></param>
        /// <param name="target"></param>
        /// <param name="preBoard">Current board pre move</param>
        /// <returns>True if SetMove is possible</returns>
        public bool IsSetMovePossible(int source, int jumped, int target, ulong preBoard)
        {
            if (!(validate(source) && validate(jumped) && validate(target))) return false;
            if (!(testBit(source, preBoard)&&testBit(jumped, preBoard)&&!testBit(target, preBoard)))return false;
            return true;  
        }

        /// <summary>
        /// Checks that a move is possible so the indexes must be valid as must the pre-conditions
        /// for the move
        /// </summary>
        /// <param name="source"></param>
        /// <param name="jumped"></param>
        /// <param name="target"></param>
        /// <param name="preBoard">Current board pre move</param>
        /// <returns>True if SetMove is possible</returns>
        public bool IsUndoMovePossible(int source, int jumped, int target, ulong preBoard)
        {
            if (!(validate(source) && validate(jumped) && validate(target))) return false;
            if (!(!testBit(source, preBoard) && !testBit(jumped, preBoard) && testBit(target, preBoard))) return false;
            return true;
        }

        /// <summary>
        /// Makes the move and updates the board and returns a ref to that board as the
        /// postBoard parameter
        /// </summary>
        /// <param name="move">The move end position</param>
        /// <param name="preBoard"></param>
        /// <param name="postBoard"></param>
        /// <returns></returns>
        public bool MakeOnBoard(
            ulong move, ref ulong preBoard, ref ulong postBoard)
        {
            postBoard = preBoard | move;
            return true;
        }

        
    }
}
