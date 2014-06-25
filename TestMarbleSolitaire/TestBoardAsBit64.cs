using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarbleSolitaire;

using System.Collections.Generic;
using System.Diagnostics;
using MarbleSolCommonLib.Common;


namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestBoardAsBit64
    {
        [TestMethod]
        public void TestBitsAreInitialised()
        {
            //arrange

            BitBoard bitBoard = new BitBoard(new List<int>() 
            { 
                0,0,1,1,1,0,0, //6
                0,0,1,1,1,0,0, //13
                1,1,1,1,1,1,1, //20
                1,1,1,1,1,1,1, //27
                1,1,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
            });

            //act
            List<Tuple<bool,bool>> testCases = new List<Tuple<bool,bool>>();
            testCases.Add(new Tuple<bool,bool>(bitBoard.IsIllegal(0),true));
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsIllegal(7*7-1), true));
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsIllegal(5-1), false));
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsIllegal(4+3*7-1), false));
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsIllegal(34-2), false));
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsIllegal(41-1), true));
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsIllegal(42), true));
            
            //assert
            int count=-1;
           
            foreach (var item in testCases)
            {
                Assert.AreEqual(item.Item1, item.Item2, "issue with"+((++count).ToString()));
            }

        }

        private BitBoard GetBitBoard()
        {
            return new BitBoard(new List<int>() 
            { 
                0,0,1,1,1,0,0, //6
                0,0,1,1,1,0,0, //13
                1,1,1,1,1,1,1, //20
                1,1,1,1,1,1,1, //27
                1,1,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
            });
        }

        [TestMethod]
        public void TestCurrentBoardInitialised()
        {
            //arrange
            BitBoard bitBoard = GetBitBoard();
            bitBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,1,0,0,0, //13
                1,1,1,1,0,0,0, //20
                1,1,1,0,1,1,1, //27
                1,1,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
            });

            Assert.AreEqual(bitBoard.BoardCurrent != 0 , true, "error in initialising current position");
        }

        [TestMethod]
        public void TestCurrentPositions()
        {
            //arrange
            BitBoard bitBoard = GetBitBoard();
            bitBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,1,0,0,0, //13
                1,1,1,1,0,0,0, //20
                0,1,1,0,1,1,1, //27
                0,0,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
            });

            //act
            List<Tuple<bool, bool>> testCases = new List<Tuple<bool, bool>>();
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsSet(0), false));
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsSet(16), true));
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsSet(39), true));
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsSet(48), false));
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsSet(42), false));
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsSet(24), false));
            testCases.Add(new Tuple<bool, bool>(bitBoard.IsSet(29), false));

            //assert
            int count = -1;

            foreach (var item in testCases)
            {
                Assert.AreEqual(item.Item1, item.Item2, "issue with" + ((++count).ToString()));
            }
        }

        [TestMethod]
        public void TestCountOfPiecesLeft()
        {
            BitBoard bitBoard = GetBitBoard();
            bitBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,0,0,0,0, //13
                1,1,1,0,0,0,0, //20
                0,1,1,1,1,1,1, //27
                0,0,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
               
            });

            bitBoard.CountPieces(bitBoard.BoardCurrent);
            int expectedCount = 2 + 1 + 3 + 6 + 5 + 3 + 3;
            Assert.AreEqual(expectedCount, bitBoard.CountPieces(bitBoard.BoardCurrent), "error in counting pieces");
        }
    }
}
