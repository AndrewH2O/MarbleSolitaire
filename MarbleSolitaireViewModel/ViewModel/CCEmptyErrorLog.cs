


using CrossCuttingLib.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireViewModel.ViewModel
{
    public class CCEmptyErrorLog:IErrorLog
    {
        public void Message(string ClassName, string ErrorId)
        {
            //swallow msgs
            throw new ArgumentException("error in game logic - setup error log to discover more");
        }
    }
}
