using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCuttingLib.Errors
{
    public class ErrorsLog:IErrorLog
    {
        public void Message(string className, string errorId)
        {
            Errors.Log(className, errorId);
        }
    }
}
