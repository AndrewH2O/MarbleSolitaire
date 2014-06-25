using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireViewModel.Moves
{
    
    public delegate void WinHandler(object sender,EventArgs e);
    public delegate void HintHandler(object sender, HintArgs e);
    public delegate void MoveHandler(object sender, MoveArgs e);
    public delegate void PiecesCountHandler(object sender, PiecesCountArgs e);
    
    public enum HintInfoState
    {
        Reset,
        Show
    }

    public class HintArgs : EventArgs
    {
        public HintInfoState HintInfoState { get; set; }
    }

    

    public class MoveArgs : EventArgs
    {
        public Object SourceJumpTarget { get; set; }
        public Action<object> UndoMove { get; set; }
        public Action<object> MakeMove { get; set; }
    }

    

    public class PiecesCountArgs : EventArgs
    {
        public int PiecesCount { get; set; }
    }

    
}
