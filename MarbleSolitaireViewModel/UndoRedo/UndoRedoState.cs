using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireViewModel.ViewModel.UndoRedo
{
    public class UndoRedoState : IUndoRedoState
    {
        readonly Action<object> _executeUndo;
        readonly Action<object> _executeRedo;
        object _state;

        public object State
        {
            get { return _state; }
            private set { _state = value; }
        }

        public UndoRedoState(Action<object> undo, Action<object> redo, object state)
        {
            if (undo == null||redo==null)
                throw new ArgumentNullException("execute");

            _executeUndo = undo;
            _executeRedo = redo;
            _state = state;
        }


        public void ExecuteUndo(object param)
        {
            _executeUndo(param);
        }

        public void ExecuteRedo(object param)
        {
            _executeRedo(param);
        }
    }
}
