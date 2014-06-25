using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{
    /// <summary>
    /// Solver contract where 
    /// LoadStart is a board of 0s and 1s, CurrentBoard is a series of bits one for
    /// each game position, IsSolution reports back the the current solution state,
    /// Solve searches for a solution and save each of the steps. GetHint get the 
    /// index of the next piece to move and the direction as an integer where
    /// 0,1,2,3 equals NSWE respectively.
    /// </summary>
    public interface ISolver
    {
        void LoadStart(List<int> start);
        
        ulong CurrentBoard { get; }
        bool IsSolution { get; }
        
        void Solve();
        bool GetHint(ref int indexPieceToMoveNext, ref int directionOfMove);
    }


    
}
