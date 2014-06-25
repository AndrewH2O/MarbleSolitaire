using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireViewModel.ViewModel.UndoRedo
{
    public class UndoRedoManager<T> : ViewModelBase where T:IUndoRedoState
    {
        #region UndoRedo

        private bool _undoRedo = false;

        /// <summary>
        /// Gets or sets the UndoRedo property. This observable property 
        /// indicates ....
        /// </summary>
        public bool UndoRedo
        {
            get { return _undoRedo; }
            set
            {
                if (_undoRedo != value)
                {
                    _undoRedo = value;
                    RaisePropertyChanged("UndoRedo");
                }
            }
        }

        #endregion


        
        bool _canUndo = false;

        public bool CanUndo
        {
            get { return _canUndo; }
            private set { _canUndo = value; }
        }

        bool _canRedo = false;

        public bool CanRedo
        {
            get { return _canRedo; }
            private set { _canRedo = value; }
        }


        private Stack<T> _undo = null;

        public Stack<T> Undo
        {
            get { return _undo; }
            private set { _undo = value; }
        }

        private Stack<T> _redo = null;

        public Stack<T> Redo
        {
            get { return _redo; }
            private set { _redo = value; }
        }

        public UndoRedoManager()
        {
            _undo = new Stack<T>();
            _redo = new Stack<T>();
        }

        bool _isInRedo;

        public bool IsInRedo
        {
            get { return _isInRedo; }
            private set { _isInRedo = value; }
        }

        /// <summary>
        /// As we perform actions for first time add to undo stack, here we record our trail
        /// </summary>
        /// <param name="undoRedo"></param>
        public void AddUndo(T undoRedo)
        {
            if (_isInRedo) return;
            _undo.Push(undoRedo);
            ClearRedo();
            updateStateFlags();
        }

        //public void UpdateOnUndoExecute()
        //{
        //    T urState = _undo.Pop();
        //    _redo.Push(urState);
        //}

        public void ExecuteUndo()
        {
            if (_undo.Count < 1) throw new InvalidOperationException("error undoAction");
            T undoRedoItem = _undo.Pop();
            undoRedoItem.ExecuteUndo(null);
            _redo.Push(undoRedoItem);
            
            updateStateFlags();
        }

        public void ExecuteRedo()
        {
            if (_redo.Count < 1) throw new InvalidOperationException("error redoAction");
            _isInRedo = true;
            T undoRedoItem = _redo.Pop();
            undoRedoItem.ExecuteRedo(null);
            _undo.Push(undoRedoItem);
            _isInRedo = false;
            updateStateFlags();
        }

        public void Reset()
        {
            _undo.Clear();
            _redo.Clear();
            updateStateFlags();
        }

        public void ClearRedo()
        {
            _redo.Clear();
            updateStateFlags();
        }


        private void updateStateFlags()
        {
            UndoRedo = !UndoRedo; //toggle so as to raise pty change events, otherwise do nothing with pty
            _canUndo = (_undo.Count > 0) ? true : false;
            _canRedo = (_redo.Count > 0) ? true : false;
        }
        
    }
}
