

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireViewModel.ViewModel
{
    public interface ISourceJumpTarget
    {
        bool IsSourceCandidate { get; set; }
        bool IsJumpCandidate { get; set; }
        bool IsTargetCandidate { get; set; }
    }

    public interface ILayout
    {
        double XPos { get; set; }
        double YPos { get; set; }
        double SideLengthPiece { get; set; }
    }
    
    /// <summary>
    /// View Piece is a piece on the board representing a legal game space;
    /// it can contain an active game piece or a space (a hole)
    /// </summary>
    public class Piece:PieceBase,ILayout,ISourceJumpTarget
    {

        #region Index

        

        /// <summary>
        /// Gets or sets the Index property. This observable property 
        /// indicates the index from the model. 
        /// </summary>
        public override int Index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    RaisePropertyChanged("Index");
                }
            }
        }

        #endregion

        

        #region View Assist with layout

        #region XPos

        private double _xPos = 0;

        /// <summary>
        /// Gets or sets the XPos property. This observable property 
        /// indicates ....
        /// </summary>
        public double XPos
        {
            get { return _xPos; }
            set
            {
                if (_xPos != value)
                {
                    _xPos = value;
                    RaisePropertyChanged("XPos");
                }
            }
        }

        #endregion

        #region YPos

        private double _yPos = 0;

        /// <summary>
        /// Gets or sets the YPos property. This observable property 
        /// indicates ....
        /// </summary>
        public double YPos
        {
            get { return _yPos; }
            set
            {
                if (_yPos != value)
                {
                    _yPos = value;
                    RaisePropertyChanged("YPos");
                }
            }
        }

        #endregion

        #region SideLengthPiece

        private double _sideLengthPiece = 0;

        /// <summary>
        /// Gets or sets the SideLengthPiece property. This observable property 
        /// indicates width and height of piece in view model coords
        /// </summary>
        public double SideLengthPiece
        {
            get { return _sideLengthPiece; }
            set
            {
                if (_sideLengthPiece != value)
                {
                    _sideLengthPiece = value;
                    RaisePropertyChanged("SideLengthPiece");
                }
            }
        }

        #endregion

        #region IsSourceCandidate

        private bool _isSourceCandidate = false;

        /// <summary>
        /// Gets or sets the IsSourceCandidate property. This observable property 
        /// indicates if Piece is a source for a jump move.
        /// </summary>
        public bool IsSourceCandidate
        {
            get { return _isSourceCandidate; }
            set
            {
                if (_isSourceCandidate != value)
                {
                    _isSourceCandidate = value;
                    RaisePropertyChanged("IsSourceCandidate");
                }
            }
        }

        #endregion



        #region IsJumpCandidate

        private bool _isJumpCandidate = false;

        /// <summary>
        /// Gets or sets the IsJumpCandidate property. This observable property 
        /// indicates if Piece can be Jumped.
        /// </summary>
        public bool IsJumpCandidate
        {
            get { return _isJumpCandidate; }
            set
            {
                if (_isJumpCandidate != value)
                {
                    _isJumpCandidate = value;
                    RaisePropertyChanged("IsJumpCandidate");
                }
            }
        }

        #endregion

        #region IsTargetCandidate

        private bool _isTargetCandidate = false;

        /// <summary>
        /// Gets or sets the IsTargetCandidate property. This observable property 
        /// indicates if is a target for a jump
        /// </summary>
        public bool IsTargetCandidate
        {
            get { return _isTargetCandidate; }
            set
            {
                if (_isTargetCandidate != value)
                {
                    _isTargetCandidate = value;
                    RaisePropertyChanged("IsTargetCandidate");
                }
            }
        }

        #endregion


        #region IsSelected

        private bool _isSelected = false;

        /// <summary>
        /// Gets or sets the IsSelected property. This observable property 
        /// indicates if piece is selected in view
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        #endregion


        

        #endregion //View Assist with layout

        public override int Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    RaisePropertyChanged("Content");
                }
            }
        }
        
        public Piece():base() {}
        
    }
}