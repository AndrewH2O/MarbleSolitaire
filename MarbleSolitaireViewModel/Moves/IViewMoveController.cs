using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace MarbleSolitaireViewModel.Moves
{
    public interface IViewMoveController
    {
        event HintHandler HintChanged;
        event MoveHandler MoveChanged;
        event WinHandler WinChanged;
        event PiecesCountHandler PiecesCountChanged;
        
        List<int> GetBoard();
        List<int> GetStart();

        void AddPiece(int XPos, int YPos, int SideLengthPiece);
        int GetPiecesCount();
        int GetSide();
        
        int GetToken(MarbleSolitaireModelLib.Model.BoardTokens token);
        
        void InitialisePiece();
        
        ObservableCollection<MarbleSolitaireViewModel.ViewModel.Piece> Pieces { get; }

        bool CheckForWin();
        bool SetupStart();
        void UpdateSelectedPiece(int currentIndex);
        bool ValidateCurrentIndex(int currentIndex);
        void ShowHintMove(bool showHide);
        bool UpdateHint(int indexNextPieceToMove, int directionOfMove);
        
        void UnsetMoveStacks();
        
        
        
    }
}
