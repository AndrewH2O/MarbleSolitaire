using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using TestMarbleSolitaire.Helpers;
using MarbleSolitaireModelLib.Model;

namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestSquareBoard
    {
        

        SquareBoard getSquareBoard()
        {
            return new SquareBoard(
                new List<int>() 
                { 
                    0,0,1,1,1,0,0,
                    0,0,1,1,1,0,0,
                    1,1,1,1,1,1,1,
                    1,1,1,1,1,1,1,
                    1,1,1,1,1,1,1,
                    0,0,1,1,1,0,0,
                    0,0,1,1,1,0,0
                }, new FakeErrorLog());
        }

        
        
        [TestMethod]
        public void TestSetupStart()
        {
            //arrange
            SquareBoard sqb = getSquareBoard();
            int expectedPiecesCount;

            //act
            sqb.SetupStart(new List<int>() 
            { 
                0,0,1,1,1,0,0,
                0,0,1,1,1,0,0,
                1,1,1,1,1,1,1,
                1,1,1,0,1,1,1,
                1,1,1,1,1,1,1,
                0,0,1,1,1,0,0,
                0,0,1,1,1,0,0
            });

            expectedPiecesCount = 4 * 6 + 8;

            //assert
            Assert.AreEqual(expectedPiecesCount, sqb.CountPieces, 
                "expectation of pieces count is wrong in testing setup start");

        }


        [TestMethod]
        public void TestIsMoveValid()
        {
            //arrange
            SquareBoard sqb = getSquareBoard();
            sqb.SetupStart(new List<int>() 
                { 
                    0,0,1,1,1,0,0,
                    0,0,1,1,1,0,0,
                    1,1,1,1,1,1,1,
                    1,1,1,0,1,1,1,
                    1,1,1,1,1,1,1,
                    0,0,1,1,1,0,0,
                    0,0,1,1,1,0,0
                }
            );

            //act+assert
            Assert.AreEqual(true, sqb.CheckMove(10, 17, 24)," expected move should be legal");
            Assert.AreEqual(true, sqb.CheckMove(22, 23, 24), "expected move should be legal");
            Assert.AreEqual(false, sqb.CheckMove(38, 39, 40), "expected move should be invalid as target illegal position");
            Assert.AreEqual(false, sqb.CheckMove(11, 10, 9), "expected move should be invalid as target is not empty");
            Assert.AreEqual(false, sqb.CheckMove(24, 23, 22), "expected move should be invalid as source is empty and target is not empty");
        }

        /*
            0	1	2	3	4	5	6
            7	8	9	10	11	12	13
            14	15	16	17	18	19	20
            21	22	23	24	25	26	27
            28	29	30	31	32	33	34
            35	36	37	38	39	40	41
            42	43	44	45	46	47	48
         */
        [TestMethod]
        public void TestIsMoveValidVariedStart()
        {
            //arrange
            SquareBoard sqb = getSquareBoard();
            sqb.SetupStart(new List<int>() 
                { 
                    0,0,0,0,0,0,0,
                    0,0,0,0,1,0,0,
                    1,1,1,0,1,1,1,
                    1,1,1,1,1,1,1,
                    1,1,1,1,1,1,1,
                    0,0,0,0,1,0,0,
                    0,0,0,1,1,0,0
                }
            );

            //act+assert
            int expectedPiecesCount =  1+6+7+7+1+2;

            Assert.AreEqual(expectedPiecesCount, sqb.CountPieces, "start setup is invalid");
            
            Assert.AreEqual(true, sqb.CheckMove(18, 11, 4), " expected move should be legal");
            Assert.AreEqual(true, sqb.CheckMove(31, 24, 17), "expected move should be legal");
            Assert.AreEqual(true, sqb.CheckMove(46, 45, 44), "expected move should be legal");
            Assert.AreEqual(false, sqb.CheckMove(2, 3, 4), "expected move should be invalid as all are not empty");
            Assert.AreEqual(false, sqb.CheckMove(29, 23, 17), "expected move should be invalid as source and jumped are in a diagonal");
            Assert.AreEqual(false, sqb.CheckMove(19, 18, 10), "expected move should be invalid as target does not match with source and jumped");
        }

        /*
            0	1	2	3	4	5	6
            7	8	9	10	11	12	13
            14	15	16	17	18	19	20
            21	22	23	24	25	26	27
            28	29	30	31	32	33	34
            35	36	37	38	39	40	41
            42	43	44	45	46	47	48
         */
        [TestMethod]
        public void TestMakeMove()
        {
            SquareBoard sqb = getSquareBoard();
            sqb.SetupStart(new List<int>() 
                { 
                    0,0,0,0,0,0,0,
                    0,0,0,0,1,0,0,
                    1,1,1,0,1,1,1,
                    1,1,1,1,1,1,1,
                    1,1,1,1,1,1,1,
                    0,0,0,0,1,0,0,
                    0,0,0,1,1,0,0
                }
            );

            //act+assert
            int expectedPiecesCount =1 + 6 + 7 + 7 + 1 + 2;
            Assert.AreEqual(expectedPiecesCount, sqb.CountPieces, "start setup is invalid");
            sqb.MakeMove(18, 11, 4);
            Assert.AreEqual(--expectedPiecesCount, sqb.CountPieces, "move successful");
            bool canMakeSameMoveAgain = false;
            Assert.AreEqual(canMakeSameMoveAgain,sqb.CheckMove(18, 11, 4),"cannot repeat move as update successful");
            sqb.MakeMove(32, 25, 18);
            Assert.AreEqual(--expectedPiecesCount, sqb.CountPieces, "move successful");
        }
    }
}
