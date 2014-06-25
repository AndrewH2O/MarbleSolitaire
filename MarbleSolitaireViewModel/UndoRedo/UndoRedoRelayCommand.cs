using MarbleSolitaireViewModel.ViewHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarbleSolitaireViewModel.ViewModel.UndoRedo
{
    public class UndoRedoRelayCommand:ICommand
    {
        bool _isUndo;

        public bool IsUndo
        {
            get { return _isUndo; }
            set { _isUndo = value; }
        }

        bool _isRedo;

        public bool IsRedo
        {
            get { return _isRedo; }
            private set { _isRedo = !_isUndo; }
        }

        

        #region Fields

        readonly Action<object> _executeUndo;
        readonly Action<object> _executeRedo;
        readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        
        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public UndoRedoRelayCommand(Action<object> executeUndo, Action<object> executeRedo, Predicate<object> canExecute)
        {
            if (executeUndo == null)
                throw new ArgumentNullException("executeUndo");
            if (executeRedo == null)
                throw new ArgumentNullException("executeRedo");
            
            _executeUndo = executeUndo;
            _executeRedo = executeRedo;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void ExecuteUndo(object parameter)
        {
            _executeUndo(parameter);
        }

        public void ExecuteRedo(object parameter)
        {
            _executeRedo(parameter);
        }

        #endregion // ICommand Members



        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
