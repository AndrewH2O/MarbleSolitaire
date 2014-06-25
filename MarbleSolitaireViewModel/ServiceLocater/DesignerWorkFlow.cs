using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarbleSolitaireModelLib.Model;
using MarbleSolitaireViewModel.ViewModel;
using System.ComponentModel;
using System.Windows;

namespace MarbleSolitaireViewModel
{
    public class DesignerWorkFlow
    {
        //register known game type tokens
        public static string BOARD = "board";
        public static string SOLVER = "solver";
        public static string ERRORLOG = "errorLog";
        
        /// <summary>
        /// Instatiates either full game objects 'registered' here if we are not in
        /// a designer or builds just the bare minimum inorder to get a working view
        /// in the designer.
        /// </summary>
        /// <param name="isInDesign">result of asking if we are in design view</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            bool isInDesignView = DesignerProperties.GetIsInDesignMode(new DependencyObject());
           
            if(isInDesignView)
            {
                if(key==BOARD) return new GameAttributes(null).GetBoardWithStart();
                if(key==SOLVER) return null;
                if(key==ERRORLOG) return null;
                return null;
            }
            else
            {
                if(key==BOARD) return new GameAttributes(new CCEmptyErrorLog()).GetBoardWithStart();
                if(key==SOLVER) return new GameAttributes(new CCEmptyErrorLog()).GetSolver2ForSquareBoardWithData();
                if(key==ERRORLOG) return new CCEmptyErrorLog();
                return null;
            }

            
        }
    }
}
