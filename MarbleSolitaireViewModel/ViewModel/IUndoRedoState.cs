using System;
namespace MarbleSolitaireViewModel.ViewModel
{
    public interface IUndoRedoState
    {
        void ExecuteRedo(object param);
        void ExecuteUndo(object param);
        object State { get; }
    }
}
