using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireViewModel.ViewModel
{
    /// <summary>
    /// Does nothing except allow us to shape the view over the data
    /// as we like to display in a grid so it is helpful to identity
    /// non valid squares and display accordingly
    /// </summary>
    public class PieceNonValid:PieceBase
    {
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

        public PieceNonValid():base(){}
    }
}
