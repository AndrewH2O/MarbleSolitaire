using MarbleSolitaireModelLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarbleSolitaire.Helpers
{
    public static class BoardFactory
    {
        static List<int> getLegalPositions()
        {
            return new List<int>() 
            { 
                0,0,1,1,1,0,0, //6
                0,0,1,1,1,0,0, //13
                1,1,1,1,1,1,1, //20
                1,1,1,1,1,1,1, //27
                1,1,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
            };
        }

        static List<int> getStart()
        {
            return new List<int>() 
            { 
                0,0,1,0,0,0,0,//6 - (0-2)
                0,0,1,0,0,0,0,//13 - (3-5)
                0,0,1,1,1,1,1,//20 - (6-12)
                0,0,0,0,1,1,1,//27 - (13-19)
                0,0,0,0,1,1,1,//34 - (20-26)
                0,0,0,0,1,0,0,//41 - (27-29)
                0,0,0,0,1,0,0//48 -  (30-32)
            };
        }
        
        public static SquareBoard GetSquareBoard()
        {
            SquareBoard sqb = new SquareBoard(
                getLegalPositions(), new FakeErrorLog());

            sqb.SetupStart(getStart());

            return sqb;
        }
    }
}
