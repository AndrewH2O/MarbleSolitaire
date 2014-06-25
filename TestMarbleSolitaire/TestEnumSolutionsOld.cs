using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarbleSolitaireLib.GameSolver;
using System.Collections.Generic;
using MarbleSolitaireModelLib.Model;
using TestMarbleSolitaire.Helpers;

namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestEnumSolutionsOld
    {
        #region setupHelpers
        

        List<int> getLegalPositions()
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

        List<int> getStart()
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



        SquareBoard getSquareBoard()
        {
            SquareBoard sqb = new SquareBoard(
                getLegalPositions(), new FakeErrorLog());

            sqb.SetupStart(getStart());

            return sqb;
        }


        RotateFlip getRotateFlip()
        {
            Mapper m = new Mapper(getSquareBoard());
            RotateFlip rf = new RotateFlip(m);
            return rf;
        }



        #endregion//setupHelpers


        //[TestMethod, Timeout(3000)]
        //[TestMethod]
        //[TestMethod, Timeout(1000)]
        [TestCategory("LongerRuntime")]
        public void TestInitEnumSolns()
        {
            RotateFlip rf = new RotateFlip(new Mapper(getSquareBoard()));
            EnumSolutionsOld es = new EnumSolutionsOld(getRotateFlip());
            Assert.IsTrue(es != null, "error init enumSolns");
        }
    }
}
