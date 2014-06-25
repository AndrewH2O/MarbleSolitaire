
using MarbleSolitaireModelLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarbleSolitaireViewModel.ViewModel
{
    public abstract class PieceBase:ViewModelBase
    {
        protected int DEFAULTCONTENT = Globals.DEFAULT_VALUE;

        public PieceBase()
        {
            _content = DEFAULTCONTENT;
        }

        #region Content

        protected int _content;

        /// <summary>
        /// Gets or sets the Content property. This observable property 
        /// indicates if piece is set.
        /// </summary>
        public abstract int Content { get;  set;}
        
        #endregion

        #region Index

        protected int _index;

        /// <summary>
        /// Gets or sets the Index property. This observable property 
        /// indicates the index position in the start arrangement of
        /// the board.
        /// </summary>
        public abstract int Index { get; set; }

        #endregion


    }
}
