using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireModelLib.Model
{
    
    
    
    public class UndoRedo
    {
        readonly Action<object> _undo;
        readonly Predicate<object> _canUndo;
        
        public void Undo(object parameter)
        {
            _undo(parameter);
        }

        public bool CanUndo(object parameter)
        {
            return _canUndo == null ? true : _canUndo(parameter);
        }
    }
}
