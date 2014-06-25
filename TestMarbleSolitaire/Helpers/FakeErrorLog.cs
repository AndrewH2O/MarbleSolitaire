

using CrossCuttingLib.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarbleSolitaire.Helpers
{
    public class FakeErrorLog:IErrorLog
    {
        public void Message(string ClassName, string ErrorId)
        {
            //I do nothing
        }
    }
}
