using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCuttingLib.Errors
{

    public interface IErrorLog
    {
        void Message(string ClassName, string ErrorId);
    }
    
    
    static public class Errors
    {
        //TODO investigate logging fw log4net

        static Dictionary<string, string> errors = new Dictionary<string, string>()
        {
            { "00001", "Unknown error occurred" },
            { "00100", "Error in setting layout" }, //game 100
            { "01000", "SquareBoard is not Square!" }, //squareBoard
            { "02000", "Game start position is not a valid subset of legal positions" },
            { "02001", "In checking move the board index was invalid"},
            { "02002", "In checking move the start index was invalid"},
            { "02003", "In checking move the jumped index was invalid"},
            { "02004", "In checking move the targets index was invalid"}
        };

        public static void Log(string className, string errId)
        {
            string errMsg = string.Empty;
            if (Errors.errors.TryGetValue(errId, out errMsg))
            {
                Debug.WriteLine(string.Format("error in {0} : {1}", className, errMsg));
            }
            else
            {
                Debug.WriteLine(string.Format("error in {0} : {1}", className, errors["00001"]));
            }

            throw new Exception("XXXXX<---  error occurred check please check log --->XXXXXXXXX");
        }
    }
}
