using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireViewModel.ViewHelpers
{

    public interface IMapModelViewSpace
    {
        int Count();
        void Add(int viewIndex, int modelIndex);
        int ConvertToViewIndex(int indexModel);
        int ConvertToModelIndex(int indexView);
    }
        

    
    
    public class MapModelViewSpace:IMapModelViewSpace
    {
        readonly int ERROR = -1;
        
        /// <summary>
        /// stores as t1 = view, t2 model
        /// </summary>
        List<ViewToModelIndex> _mapViewIndexToModel = new List<ViewToModelIndex>();

        private List<ViewToModelIndex> mapViewIndexToModel
        {
            get { return _mapViewIndexToModel; }
            set { _mapViewIndexToModel = value; }
        }

        
        public MapModelViewSpace()
        {
            _mapViewIndexToModel = new List<ViewToModelIndex>();
        }


        public int Count()
        {
            return _mapViewIndexToModel.Count;
        }


        /// <summary>
        /// Add Index mapping
        /// </summary>
        /// <param name="viewIndex">view index</param>
        /// <param name="modelIndex">model index</param>
        public void Add(int viewIndex, int modelIndex)
        {
            _mapViewIndexToModel.Add(new ViewToModelIndex()
                            {
                                ViewIndex = viewIndex,
                                ModelIndex = modelIndex
                            }
                        );
        }

        /// <summary>
        /// Converts from model coords to view
        /// </summary>
        /// <param name="indexModel">index in model</param>
        /// <returns>view index can return ERROR</returns>
        public int ConvertToViewIndex(int indexModel)
        {
            foreach (var item in _mapViewIndexToModel)
            {
                if (indexModel == item.ModelIndex) return item.ViewIndex;
            }

            return ERROR;
        }


        public int ConvertToModelIndex(int indexView)
        {
            return _mapViewIndexToModel[indexView].ModelIndex;
        }
    }
}
