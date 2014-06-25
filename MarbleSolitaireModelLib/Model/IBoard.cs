using System;
using System.Collections.Generic;
namespace MarbleSolitaireModelLib.Model
{
    public enum BoardTokens
    {
        TokenHasPiece,
        TokenIllegalPosition,
        TokenIsSpace, 
        TokenUnknown 
    }
    
    
    public interface IBoard
    {
        int TokenHasPiece { get; }
        int TokenIllegalPosition { get; }
        int TokenIsSpace { get; }
        int TokenUnknown { get; }

        

        List<int> BoardItems { get; }
        List<int> Start { get; set; }
        
        void SetupStart(List<int> start);
        
        int[] GetListOfJumpedCandidates(int index);
        int[] GetListOfTargetCandidates(int index);
        int[] GetListOfSourceCandidates(int index);

        void MakeMove(int start, int jumped, int target);
        void UnMakeMove(int start, int jumped, int target);
        bool CheckMove(int start, int jumped, int target);
        bool CheckUndoMove(int start, int jumped, int target);
    }
}
