using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using TestMarbleSolitaire.Helpers;
using MarbleSolitaireModelLib.Model;
using MarbleSolitaireLib.GameSolver;


namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestMapper
    {
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
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            };
        }
        
        SquareBoard getSquareBoard()
        {
            SquareBoard sqb = new SquareBoard(
                getLegalPositions(), new FakeErrorLog());

            sqb.SetupStart(getStart());

            return sqb;
        }
        
        
        
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestMappers()
        {
            Mapper mapper = new Mapper(getSquareBoard());
            int NON_LEGAL = -1;

            Assert.AreEqual(33, mapper.MapIndexGameToModel.Count, "expected count GameToModel in error");
            Assert.AreEqual(33, mapper.MapIndexGameToModel.Count, "expected count ModelToGame in error");
            Assert.AreEqual(11, mapper.MapIndexGameToModel[5], "expected mapping game index to model in error 5->11");
            Assert.AreEqual(24, mapper.MapIndexGameToModel[16], "expected mapping game index to model in error 16->24");
            Assert.AreEqual(46, mapper.MapIndexGameToModel[32], "expected mapping game index to model in error 32->46");

            Assert.AreEqual(-1, mapper.MapIndexModelToGame[0], 
                "expected mapping model index is non legal to game in error 0->-1 throws keynotfoundExcpn");
            Assert.AreEqual(-1, mapper.MapIndexModelToGame[7 * 7 - 1], 
                "expected mapping model index is non legal to game in error 48->-1 throws keynotfoundExcpn");
            
            Assert.AreEqual(5, mapper.MapIndexModelToGame[11], "expected mapping valid model index to game in error 11->5");
            Assert.AreEqual(16, mapper.MapIndexModelToGame[24], "expected mapping valid model index to game in error 24->16");
            Assert.AreEqual(32, mapper.MapIndexModelToGame[46], "expected mapping valid model index to game in error 46->32");

            Assert.AreEqual(NON_LEGAL, mapper.GetModelToGameByIndex(0), "expected mapping model index is non legal to game in error 0->-1");
            Assert.AreEqual(NON_LEGAL, mapper.GetModelToGameByIndex(7 * 7 - 1), "expected mapping model index is non legal to game in error 48->-1");
        }
    }
}
