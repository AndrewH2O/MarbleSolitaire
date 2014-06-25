#define SOURCE_HAS_TEST_DEF
//#undef SOURCE_HAS_TEST_DEF
#define STORAGE
//#undef STORAGE
//#define LONGRUNNINGON
//#undef LONGRUNNINGON


//
//To test everything 
//comment out #undef SOURCE_HAS_TEST_DEF above and #undef TEST in source
//to #define SOURCE_HAS_TEST_DEF and TEST in unit test and source code;
//Use both #define and #undef as a toggle pair to make it 
//more explicit (easy to miss a comment otherwise). 
//To test higher granularity behavior leave #undef in place both here and in
//the source
//which deselects lower level functionality and cleans the source interface.
//Source has public accessors which are marked with TEST which if undefined
//breaks unit tests (they don't compile) so use with SOURCE_HAS_TEST_DEF 
//#define and #undef to deselect.  
//Avoids testing private field and method accessors or using reflection 
//and makes it explicit which is the higher level functionality.
//
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using TestMarbleSolitaire.Helpers;

using System.Text;
using MarbleSolitaireModelLib.Model;
using MarbleSolitaireLib.GameSolver;
using MarbleSolitaireLib.Data;

namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestSolver2
    {
        #region setupHelpers
        Helpers.BitHelpers _bitHelpers = new BitHelpers();

        EnumSolutionsDTO getEnumSolutionsDTO()
        {
            bool isError = false;
            EnumSolutionsDTO dto =
                (EnumSolutionsDTO)Data.DataHelper.GetOrCreateBinary<IEnumSolutionsDTO>("EnumDto.dat", out isError);
            Assert.IsFalse(isError, "data loaded ok");
            return dto;
        }

        #endregion//setupHelpers

        [TestMethod]
        public void TestInitSolver2()
        {
            Solver2 solver2 = new Solver2(Helpers.BoardFactory.GetSquareBoard());
            Assert.IsTrue(solver2 != null, "error in init solver 2");
        }

        [TestMethod]
        public void TestLoadingStartBoardWithString()
        {
            Solver2 solver2 = new Solver2(Helpers.BoardFactory.GetSquareBoard());
            //length 49
            string getStart =
                "0,0,1,1,1,0,0" +    //6 - (0-2)
                "0,0,1,1,1,0,0" +    //13 - (3-5)
                "1,1,1,1,1,1,1" +    //20 - (6-12)
                "1,1,1,0,1,1,1" +    //27 - (13-19)
                "1,1,1,1,1,1,1" +    //34 - (20-26)
                "0,0,1,1,1,0,0" +    //41 - (27-29)
                "0,0,1,1,1,0,0";     //48 -  (30-32)

            solver2.LoadBoard(getStart,LoadState.Start);
            

            //string leadingPacking = "0000 0000 0000 0000";
            //string strGameBoardPos = "0000 0000 0000 0000" + "0000 0000 0000 0000" + "0";
            //string strMoveIDPacking = "000 000 000";
            //string strPiecesCountPacking = "000 000";
            StorageBitPacker sbp = new StorageBitPacker();

            getStart = convertModelToGameString(getStart);
            
            ulong expected = expectedBitsValue(getStart,null,null);
            Assert.IsTrue(solver2.StartBoard != 0, "Current board has a value in error");
            Assert.IsTrue(solver2.CurrentBoard == solver2.StartBoard, "loading board with start flag sets current board in error");
            Assert.AreEqual(expected, solver2.StartBoard, 
                "solver2 currentBoard (length of input 49) in error");
            Assert.AreEqual(32, solver2.PiecesCount, "Pieces count in error");
            
            //length 33
            getStart =
                    "1,1,1," +    //6 - (0-2)
                    "1,1,1," +    //13 - (3-5)
                "1,1,1,1,1,1,1" +    //20 - (6-12)
                "1,1,1,0,1,1,1" +    //27 - (13-19)
                "1,1,1,1,1,1,1" +    //34 - (20-26)
                    "1,1,1," +    //41 - (27-29)
                    "1,1,1,";     //48 -  (30-32)

            solver2.LoadBoard(getStart,LoadState.Start);
            
            Assert.AreEqual(expected, solver2.StartBoard,
                "solver2 currentBoard (length of input 33) in error");
            Assert.AreEqual(32, solver2.PiecesCount, "Pieces count in error");
        }

        [TestMethod]
        public void TestLoadingCurrentBoardWithString()
        {
            Solver2 solver2 = new Solver2(Helpers.BoardFactory.GetSquareBoard());
            
            //length 49
            string getCurrent =
                "0,0,1,1,1,0,0" +    //6 - (0-2)
                "0,0,1,1,1,0,0" +    //13 - (3-5)
                "1,1,1,1,1,0,0" +    //20 - (6-12)
                "1,1,1,0,1,0,0" +    //27 - (13-19)
                "1,1,1,1,1,0,0" +    //34 - (20-26)
                "0,0,0,0,0,0,0" +    //41 - (27-29)
                "0,0,0,0,0,0,0";     //48 -  (30-32)

            solver2.LoadBoard(getCurrent,LoadState.Current);


            //string leadingPacking = "0000 0000 0000 0000";
            //string strGameBoardPos = "0000 0000 0000 0000" + "0000 0000 0000 0000" + "0";
            //string strMoveIDPacking = "000 000 000";
            //string strPiecesCountPacking = "000 000";
            StorageBitPacker sbp = new StorageBitPacker();

            getCurrent = convertModelToGameString(getCurrent);

            ulong expected = expectedBitsValue(getCurrent, null, null);
            Assert.IsTrue(solver2.CurrentBoard != 0, "Current board has a value in error");

            Assert.AreEqual(expected, solver2.CurrentBoard,
                "solver2 currentBoard (length of input = 49) in error");
            Assert.AreEqual(20, solver2.PiecesCount, "Pieces count in error");
            
            //length 33
            getCurrent =
                    "1,1,1," +    //6 - (0-2)
                    "1,1,1," +    //13 - (3-5)
                "1,1,1,1,1,0,0" +    //20 - (6-12)
                "1,1,1,0,1,0,0" +    //27 - (13-19)
                "1,1,1,1,1,0,0" +    //34 - (20-26)
                    "0,0,0," +    //41 - (27-29)
                    "0,0,0,";     //48 -  (30-32)

            solver2.LoadBoard(getCurrent,LoadState.Current);
            Assert.AreEqual(expected, solver2.CurrentBoard,
                "solver2 currentBoard (length of input = 33) in error");
            Assert.AreEqual(20, solver2.PiecesCount, "Pieces count in error");
        }

        [TestMethod]
        public void TestLoadingStartBoardWithListInt()
        {
            Solver2 solver2 = new Solver2(Helpers.BoardFactory.GetSquareBoard());
            
            //length 49
            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            };

            solver2.LoadBoard(getStart,LoadState.Start);

            //string leadingPacking = "0000 0000 0000 0000";
            //string strGameBoardPos = "0000 0000 0000 0000" + "0000 0000 0000 0000" + "0";
            //string strMoveIDPacking = "000 000 000";
            //string strPiecesCountPacking = "000 000";
            StorageBitPacker sbp = new StorageBitPacker();

            getStart = convertModelToGameListInt(getStart);

            string getExpectedString =
                    "1,1,1," +    //6 - (0-2)
                    "1,1,1," +    //13 - (3-5)
                "1,1,1,1,1,1,1" +    //20 - (6-12)
                "1,1,1,0,1,1,1" +    //27 - (13-19)
                "1,1,1,1,1,1,1" +    //34 - (20-26)
                    "1,1,1," +    //41 - (27-29)
                    "1,1,1,";     //48 -  (30-32)

            ulong expected = expectedBitsValue(getExpectedString, null, null);
            Assert.IsTrue(solver2.StartBoard != 0, "Current board has a value in error");
            Assert.IsTrue(solver2.CurrentBoard == solver2.StartBoard, "loading board with start flag sets current board in error");
            Assert.AreEqual(expected, solver2.StartBoard,
                "solver2 currentBoard (length of input = 49) in error");
            Assert.AreEqual(32, solver2.PiecesCount, "Pieces count in error");

            //length 33
            getStart = new List<int>() 
            { 
                    1,1,1,//6 - (0-2)
                    1,1,1,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                    1,1,1,//41 - (27-29)
                    1,1,1//48 -  (30-32)
            };

            solver2.LoadBoard(getStart,LoadState.Start);
            
            Assert.AreEqual(expected, solver2.StartBoard,
                "solver2 currentBoard (length of input = 33) in error");
            Assert.AreEqual(32, solver2.PiecesCount, "Pieces count in error");
        }


        [TestMethod]
        public void TestLoadingCurrentBoardWithListInt()
        {
            Solver2 solver2 = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            List<int> getCurrent = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,0,0,//20 - (6-12)
                1,1,1,0,1,0,0,//27 - (13-19)
                1,1,1,1,1,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0//48 -  (30-32)
            };

            solver2.LoadBoard(getCurrent,LoadState.Current);

            //string leadingPacking = "0000 0000 0000 0000";
            //string strGameBoardPos = "0000 0000 0000 0000" + "0000 0000 0000 0000" + "0";
            //string strMoveIDPacking = "000 000 000";
            //string strPiecesCountPacking = "000 000";
            StorageBitPacker sbp = new StorageBitPacker();

            getCurrent = convertModelToGameListInt(getCurrent);

            string getExpectedString =
                    "1,1,1," +    //6 - (0-2)
                    "1,1,1," +    //13 - (3-5)
                "1,1,1,1,1,0,0" +    //20 - (6-12)
                "1,1,1,0,1,0,0" +    //27 - (13-19)
                "1,1,1,1,1,0,0" +    //34 - (20-26)
                    "0,0,0," +    //41 - (27-29)
                    "0,0,0,";     //48 -  (30-32)

            ulong expected = expectedBitsValue(getExpectedString, null, null);
            Assert.IsTrue(solver2.CurrentBoard != 0, "Current board has a value in error");

            Assert.AreEqual(expected, solver2.CurrentBoard,
                "solver2 currentBoard (length of input = 49) in error");
            Assert.AreEqual(20, solver2.PiecesCount, "Pieces count in error");
            
            getCurrent = new List<int>() 
            { 
                    1,1,1,//6 - (0-2)
                    1,1,1,//13 - (3-5)
                1,1,1,1,1,0,0,//20 - (6-12)
                1,1,1,0,1,0,0,//27 - (13-19)
                1,1,1,1,1,0,0,//34 - (20-26)
                    0,0,0,//41 - (27-29)
                    0,0,0//48 -  (30-32)
            };

            solver2.LoadBoard(getCurrent,LoadState.Current);
            Assert.AreEqual(expected, solver2.CurrentBoard,
                "solver2 currentBoard (length of input = 33) in error");
            Assert.AreEqual(20, solver2.PiecesCount, "Pieces count in error");
        }

        string convertModelToGameString(string board)
        {
            board = StorageBitPacker.CleanString(board);
            StringBuilder sb = new StringBuilder();
            Mapper m = new Mapper(Helpers.BoardFactory.GetSquareBoard());
            for (int i = 0; i < board.Length; i++)
            {
                int index = m.GetModelToGameByIndex(i);
                if (index != m.NON_LEGAL)
                {
                    sb.Append(board[i]);
                }
            }
            return sb.ToString();
        }

        List<int> convertModelToGameListInt(List<int> board)
        {

            List<int> l = new List<int>();
            Mapper m = new Mapper(Helpers.BoardFactory.GetSquareBoard());
            for (int i = 0; i < board.Count; i++)
            {
                int index = m.GetModelToGameByIndex(i);
                if (index != m.NON_LEGAL)
                {
                    l.Add(board[i]);
                }
            }
            return l;
        }

        

        [TestMethod]
        public void TestFindAvailableMovesOnGameBoard()
        {
            Solver2 solver2 = new Solver2(Helpers.BoardFactory.GetSquareBoard());
            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            };

            solver2.LoadBoard(getStart,LoadState.Start);

            ulong currentBoard = solver2.CurrentBoard;

            ushort piecesCount = 32;
            
            solver2.FindAvailableMoves(currentBoard, piecesCount);
            
            Assert.IsTrue(solver2.AvailableMovesFound() != solver2.ERROR, "Number moves found on board not in error");
            
            int expectedAvailableMoveCount = 4;
            Assert.AreEqual(expectedAvailableMoveCount, solver2.AvailableMovesFound(), "Error in number moves available on current board");
        }


        [TestMethod]
        public void TestFindAvailableMovesOnGameBoard2()
        {
            Solver2 solver2 = new Solver2(Helpers.BoardFactory.GetSquareBoard());
            string getStart =  
             
                		"1	1	1"+		
		                "0	1	1"+		
                "0	1	1	1	0	0	0"+
                "0	1	1	0	1	0	0"+
                "0	1	0	1	1	1	0"+
		                "0	0	1"+		
		                "0	0	0"	;	


            solver2.LoadBoard(getStart,LoadState.Current);

            ulong currentBoard = solver2.CurrentBoard;

            ushort piecesCount = 16;

            solver2.FindAvailableMoves(currentBoard, piecesCount);

            Assert.IsTrue(solver2.AvailableMovesFound() != solver2.ERROR, "Number moves found on board not in error");
            /*
                moves available						
						
		                0	0	1		
		                0	1	1		
                0	0	3	0	0	0	0
                0	1	2	0	0	0	0
                0	0	0	0	4	0	0
		                0	0	0		
		                0	0	0		
						                13

             */

            int expectedAvailableMoveCount = 13;
            Assert.AreEqual(expectedAvailableMoveCount, solver2.AvailableMovesFound(), "Error in number moves available on current board");
        }

        [TestMethod]
        public void TestMakeMove()
        {
            Solver2 solver2 = new Solver2(Helpers.BoardFactory.GetSquareBoard());
            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            };

            solver2.LoadBoard(getStart,LoadState.Start);
            ulong actualBoard=solver2.CurrentBoard;
            actualBoard= solver2.MakeMove(actualBoard, 3 * 100 + 14);

            List<int> expectedPostMove = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,0,0,1,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            };
            solver2.LoadBoard(expectedPostMove,LoadState.Start);
            ulong expectedBoard = solver2.CurrentBoard;
            Assert.AreEqual(expectedBoard, actualBoard, "Error in making move");
        }

        [TestMethod]
        public void TestUpdateSolutionProgress()
        {
            Solver2 solver2 = new Solver2(Helpers.BoardFactory.GetSquareBoard());
            //solver2.LoadBoard(getStart(),LoadState.Start);
            ushort piecesCount = 20 - 1- 1;
            ushort moveID = 2 * 100 + 8;
            ulong board = 2481925714;
            ulong expectedStorageBits = 80954025520722;

            
            ulong actualStorageBits = solver2.UpdateData(board, moveID, piecesCount);
            Assert.AreEqual(expectedStorageBits, actualStorageBits, "UpdateSoln in error");

        }



        [TestMethod]
        public void TestMake5Moves()
        {
            //arrange
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            };

            solver.LoadBoard(getStart, LoadState.Start);
            int numberOfMovesToBailOn = 5;
            solver.Solve(numberOfMovesToBailOn);
            
            //act
            ulong actualBoardAfter5Moves = solver.NextBoard;
            
            //arrange: use solver to convert board to ulong
            solver.LoadBoard(new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,1,1,1,1,//27 - (13-19)
                1,1,1,0,0,1,1,//34 - (20-26)
                0,0,1,0,0,0,0,//41 - (27-29)
                0,0,0,0,1,0,0//48 -  (30-32)
            },LoadState.Current);

            ulong expectedBoard = solver.CurrentBoard;
            Assert.AreEqual(expectedBoard, actualBoardAfter5Moves, "After 5 Moves");
        }


        

        [TestMethod]
        public void TestMake15Moves()
        {
            //arrange
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            };

            solver.LoadBoard(getStart, LoadState.Start);
            int numberOfMovesToBailOn = 15;
            solver.Solve(numberOfMovesToBailOn);
            
            //act
            ulong actualBoardAfter15Moves = solver.NextBoard;
            
            //arrange: use solver to convert board to ulong
            solver.LoadBoard(new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                0,0,0,0,1,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,1,0,1,0,0,//41 - (27-29)
                0,0,1,0,0,0,0//48 -  (30-32)
            }, LoadState.Current);

            ulong expectedBoard = solver.CurrentBoard;
            Assert.AreEqual(expectedBoard, actualBoardAfter15Moves, "After 15 Moves");
        }


        [TestMethod]
        public void TestMake26Moves()
        {
            //arrange
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            };

            solver.LoadBoard(getStart, LoadState.Start);
            int numberOfMovesToBailOn = 26;
            solver.Solve(numberOfMovesToBailOn);

            //act
            ulong actualBoardAfter26Moves = solver.NextBoard;
            
            //arrange: use solver to convert board to ulong
            solver.LoadBoard(new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,1,0,0,0,0,//20 - (6-12)
                0,0,0,0,1,0,0,//27 - (13-19)
                0,0,1,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0//48 -  (30-32)
            }, LoadState.Current);

            ulong expectedBoard = solver.CurrentBoard;
            
            Assert.AreEqual(expectedBoard, actualBoardAfter26Moves, "After 26 Moves");
        }

        [TestMethod]
        public void TestSolutionFromPartialBoard()
        {
            //arrange
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,0,0,1,0,0,//6 - (0-2)
                0,0,0,0,1,0,0,//13 - (3-5)
                0,0,1,1,1,0,0,//20 - (6-12)
                0,0,0,1,1,0,0,//27 - (13-19)
                0,1,1,1,1,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };

            List<int> getWin = new List<int>()
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };

            solver.LoadBoard(getStart, LoadState.Start);
            solver.LoadBoard(getWin, LoadState.Win);
            
            solver.Solve();
            bool ExpectedIsSolution = true;
            bool ActualIsSolution = solver.IsSolution;
            //act
            ulong actualBoard = solver.NextBoard;
            
            //arrange: use solver to convert board to ulong
            solver.LoadBoard(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            }, LoadState.Current);

            ulong expectedBoard = solver.CurrentBoard;
            Assert.AreEqual(ExpectedIsSolution, ActualIsSolution, "IsSolution expectation in error");
            Assert.AreEqual(expectedBoard, actualBoard, "win from partial start board in error");
            
        }

        [TestMethod]
        public void TestSolutionFromCompleteBoard()
        {
            //arrange
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            };

            List<int> getWin = new List<int>()
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };

            solver.LoadBoard(getStart, LoadState.Start);
            solver.LoadBoard(getWin, LoadState.Win);

            solver.Solve();
            bool ExpectedIsSolution = true;
            bool ActualIsSolution = solver.IsSolution;
            //act
            ulong actualBoard = solver.NextBoard;

            //arrange: use solver to convert board to ulong
            solver.LoadBoard(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            }, LoadState.Current);

            ulong expectedBoard = solver.CurrentBoard;

            Assert.AreEqual(expectedBoard, actualBoard, "win from complete start board in error");
            Assert.AreEqual(ExpectedIsSolution, ActualIsSolution, "IsSolution expectation in error");
        }

        [TestMethod]
        public void TestNoSolutionFound()
        {
            //arrange
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,1,0,0,0,0,//6 - (0-2)
                0,0,1,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,1,1,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                1,1,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,1,0,0,//41 - (27-29)
                0,0,0,0,1,0,0//48 -  (30-32)
            };

            List<int> getWin = new List<int>()
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };

            solver.LoadBoard(getStart, LoadState.Start);
            solver.LoadBoard(getWin, LoadState.Win);

            solver.Solve();
            bool ExpectedIsSolution = false;
            bool ActualIsSolution = solver.IsSolution;
            //act
            ulong actualBoard = solver.NextBoard;
#if STORAGE
            //arrange: use solver to convert board to ulong
            solver.LoadBoard(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,1,0,0,1,1,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                1,1,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,1,0,0,//41 - (27-29)
                0,0,0,0,1,0,0 //48 -  (30-32)
            }, LoadState.Current);

            ulong expectedBoard = solver.CurrentBoard;
            Assert.AreEqual(expectedBoard, actualBoard, "No solution found end board");
#else
            //arrange: use solver to convert board to ulong
            solver.LoadBoard(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,1,0,1,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,1,0,1,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            }, LoadState.Current);

            ulong expectedBoard = solver.CurrentBoard;
            Assert.AreEqual(expectedBoard, actualBoard, "No solution found end board");
#endif
            Assert.AreEqual(ExpectedIsSolution, ActualIsSolution, "IsSolution expectation in error");
        }

        [TestMethod]
        public void TestLoadAsUlongFromPartialWithSoln()
        {
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            ulong completeStart = 31655716;
            ulong win = 65536;

            solver.LoadBoard(completeStart, LoadState.Start);
            solver.LoadBoard(win, LoadState.Win);

            solver.Solve();
            bool ExpectedIsSolution = true;
            bool ActualIsSolution = solver.IsSolution;
            //act
            ulong actualBoard = solver.NextBoard;

            //arrange: use solver to convert board to ulong
            solver.LoadBoard(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            }, LoadState.Current);

            ulong expectedBoard = solver.CurrentBoard;

            Assert.AreEqual(expectedBoard, actualBoard, "win from partial start board in error loading from ulong");
            Assert.AreEqual(ExpectedIsSolution, ActualIsSolution, "IsSolution expectation in error loading from a ulong");
        }


        [TestMethod]
        public void TestLoadAsUlongFromCompleteWithSoln()
        {
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            ulong completeStart = 8589869055;
            ulong win = 65536;

            solver.LoadBoard(completeStart, LoadState.Start);
            solver.LoadBoard(win, LoadState.Win);

            solver.Solve();
            bool ExpectedIsSolution = true;
            bool ActualIsSolution = solver.IsSolution;
            //act
            ulong actualBoard = solver.NextBoard;

            //arrange: use solver to convert board to ulong
            solver.LoadBoard(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            }, LoadState.Current);

            ulong expectedBoard = solver.CurrentBoard;

            Assert.AreEqual(expectedBoard, actualBoard, "win from complete start board in error loading from ulong");
            Assert.AreEqual(ExpectedIsSolution, ActualIsSolution, "IsSolution expectation in error loading from a ulong");
        }

        [TestMethod]
        public void TestLoadAsUlongNoSoln()
        {
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            ulong completeStart = 4835055625;
            ulong win = 65536;

            solver.LoadBoard(completeStart, LoadState.Start);
            solver.LoadBoard(win, LoadState.Win);

            solver.Solve();
            bool ExpectedIsSolution = false;
            bool ActualIsSolution = solver.IsSolution;
            //act
            ulong actualBoard = solver.NextBoard;

            //arrange: use solver to convert board to ulong
            solver.LoadBoard(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,1,0,0,1,1,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                1,1,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,1,0,0,//41 - (27-29)
                0,0,0,0,1,0,0 //48 -  (30-32)
            }, LoadState.Current);

            ulong expectedBoard = solver.CurrentBoard;

            Assert.AreEqual(expectedBoard, actualBoard, "no win expected in error loading from ulong");
            Assert.AreEqual(ExpectedIsSolution, ActualIsSolution, "IsSolution expectation in error loading from a ulong");
        }

#if LONGRUNNINGON

        //[TestMethod]
        //[TestMethod, Timeout(7000)]
        [TestMethod]
        [TestCategory("LongerRuntime")]
        public void TestUnknownSolution()
        {
            //arrange
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());
            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,
                0,0,0,1,1,0,0,
                1,1,0,1,1,1,1,
                1,0,1,0,1,1,1,
                1,1,1,1,1,1,1,
                0,0,1,1,1,0,0,
                0,0,1,1,1,0,0
            };
            //List<int> getStart = new List<int>() 
            //{ 
            //    0,0,1,1,1,0,0,//6 - (0-2)
            //    0,0,1,1,1,0,0,//13 - (3-5)
            //    1,1,1,1,1,1,1,//20 - (6-12)
            //    1,1,1,0,1,0,1,//27 - (13-19)
            //    1,1,1,1,0,1,1,//34 - (20-26)
            //    0,0,1,1,0,0,0,//41 - (27-29)
            //    0,0,1,1,1,0,0 //48 -  (30-32)
            //};

            List<int> getWin = new List<int>()
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };

            solver.LoadBoard(getStart, LoadState.Start);
            solver.LoadBoard(getWin, LoadState.Win);

            solver.Solve();
            bool ExpectedIsSolution = false;
            bool ActualIsSolution = solver.IsSolution;
            //act
            ulong actualBoard = solver.NextBoard;


            Assert.AreEqual(ExpectedIsSolution, ActualIsSolution, "IsSolution expectation in error");
        }
#endif

        //[TestMethod]
        //[TestMethod, Timeout(4000)]
        [TestMethod]
        public void TestSolutionWhichTakesTimeToFind()
        {
            //arrange
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            //List<int> getStart = new List<int>() 
            //{ 
            //    0,0,1,1,1,0,0,//6 - (0-2)
            //    0,0,0,1,1,0,0,//13 - (3-5)
            //    1,1,0,1,1,1,1,//20 - (6-12)
            //    1,0,1,1,1,1,1,//27 - (13-19)
            //    1,1,1,1,1,1,1,//34 - (20-26)
            //    0,0,1,1,1,0,0,//41 - (27-29)
            //    0,0,1,1,1,0,0//48 -  (30-32)
            //};
            List<int> getStart = new List<int>() 
            { 
                0,0,0,1,1,0,0,//6 - (0-2)
                0,0,0,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,0,0,1,1,1,1,//27 - (13-19)
                0,0,0,1,1,1,1,//34 - (20-26)
                0,0,0,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            };

            List<int> getWin = new List<int>()
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };

            solver.LoadBoard(getStart, LoadState.Start);
            solver.LoadBoard(getWin, LoadState.Win);

            solver.Solve();
            bool ExpectedIsSolution = true;
            bool ActualIsSolution = solver.IsSolution;
            //act
            ulong actualBoard = solver.NextBoard;

            //arrange: use solver to convert board to ulong
            solver.LoadBoard(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            }, LoadState.Current);

            ulong expectedBoard = solver.CurrentBoard;

            Assert.AreEqual(expectedBoard, actualBoard, "win from complete start board in error");
            Assert.AreEqual(ExpectedIsSolution, ActualIsSolution, "IsSolution expectation in error");
        }

#if LONGRUNNINGON 
        //[TestMethod, Timeout(3000)]
        //[TestMethod]
        [TestMethod]
        public void TestUnknownSolutionWithDataLookup()
        {
            //arrange
            EnumSolutionsDTO dto = getEnumSolutionsDTO();

            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard(), dto);
            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,
                0,0,0,1,1,0,0,
                1,1,0,1,1,1,1,
                1,0,1,0,1,1,1,
                1,1,1,1,1,1,1,
                0,0,1,1,1,0,0,
                0,0,1,1,1,0,0
            };
            //List<int> getStart = new List<int>() 
            //{ 
            //    0,0,1,1,1,0,0,//6 - (0-2)
            //    0,0,1,1,1,0,0,//13 - (3-5)
            //    1,1,1,1,1,1,1,//20 - (6-12)
            //    1,1,1,0,1,0,1,//27 - (13-19)
            //    1,1,1,1,0,1,1,//34 - (20-26)
            //    0,0,1,1,0,0,0,//41 - (27-29)
            //    0,0,1,1,1,0,0 //48 -  (30-32)
            //};

            List<int> getWin = new List<int>()
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };

            solver.LoadBoard(getStart, LoadState.Start);
            solver.LoadBoard(getWin, LoadState.Win);



            solver.Solve();
            bool ExpectedIsSolution = false;
            bool ActualIsSolution = solver.IsSolution;
            //act
            ulong actualBoard = solver.NextBoard;


            Assert.AreEqual(ExpectedIsSolution, ActualIsSolution, "IsSolution expectation in error");
        }

#endif //LONGRUNNINGON

        [TestMethod]
        public void TestHintWithSolutionFromPartialBoard()
        {
            //arrange
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,0,0,1,0,0,//6 - (0-2)
                0,0,0,0,1,0,0,//13 - (3-5)
                0,0,1,1,1,0,0,//20 - (6-12)
                0,0,0,1,1,0,0,//27 - (13-19)
                0,1,1,1,1,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };

            List<int> getWin = new List<int>()
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };

            solver.LoadBoard(getStart, LoadState.Current);
            solver.LoadBoard(getWin, LoadState.Win);

            solver.Solve();
            bool ExpectedIsSolution = true;
            bool ActualIsSolution = solver.IsSolution;
            //act
            ulong actualBoard = solver.NextBoard;

            int indexPieceToMoveNext = -1;
            int directionOfMove = -1;
            solver.GetHint(ref indexPieceToMoveNext, ref directionOfMove);

            //arrange: use solver to convert board to ulong
            solver.LoadBoard(65536,LoadState.Win);
            ulong expectedBoard = solver.WinBoard;
            Assert.AreEqual(ExpectedIsSolution, ActualIsSolution, "IsSolution expectation in error");
            Assert.AreEqual(expectedBoard, actualBoard, "win from partial start board in error");
            Assert.AreEqual(23, indexPieceToMoveNext, "index of piece error");
            Assert.AreEqual(3, directionOfMove, "NSWE index 0123 direction of move error error");
           
        }

        [TestMethod]
        public void TestHintWithSolutionFromPartialBoardToWin()
        {
            //arrange
            Solver2 solver = new Solver2(Helpers.BoardFactory.GetSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,0,0,1,0,0,//6 - (0-2)
                0,0,0,0,1,0,0,//13 - (3-5)
                0,0,1,1,1,0,0,//20 - (6-12)
                0,0,0,1,1,0,0,//27 - (13-19)
                0,1,1,1,1,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };

            List<int> getWin = new List<int>()
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };

            solver.LoadBoard(getStart, LoadState.Current);
            solver.LoadBoard(getWin, LoadState.Win);
            solver.Solve();
            
            int indexPieceToMoveNext = -1;
            int directionOfMove = -1;
            solver.GetHint(ref indexPieceToMoveNext, ref directionOfMove);
            Assert.AreEqual(23, indexPieceToMoveNext, "index of piece error");
            Assert.AreEqual(3, directionOfMove, "NSWE index 0123 direction of move error error");

            solver.LoadBoard(new List<int>()
                { 
                    0,0,0,0,1,0,0,//6 - (0-2)
                    0,0,0,0,1,0,0,//13 - (3-5)
                    0,0,1,1,1,0,0,//20 - (6-12)
                    0,0,0,1,1,0,0,//27 - (13-19)
                    0,1,1,0,0,1,0,//34 - (20-26)
                    0,0,0,0,0,0,0,//41 - (27-29)
                    0,0,0,0,0,0,0 //48 -  (30-32)
                },
                LoadState.Current);
            solver.Solve();
            solver.GetHint(ref indexPieceToMoveNext, ref directionOfMove);
            Assert.AreEqual(21, indexPieceToMoveNext, "index of piece error");
            Assert.AreEqual(3, directionOfMove, "NSWE index 0123 direction of move error error");

            solver.LoadBoard(new List<int>()
                { 
                    0,0,0,0,1,0,0,//6 - (0-2)
                    0,0,0,0,1,0,0,//13 - (3-5)
                    0,0,1,1,1,0,0,//20 - (6-12)
                    0,0,0,1,1,0,0,//27 - (13-19)
                    0,0,0,1,0,1,0,//34 - (20-26)
                    0,0,0,0,0,0,0,//41 - (27-29)
                    0,0,0,0,0,0,0 //48 -  (30-32)
                },
                LoadState.Current);
            solver.Solve();
            solver.GetHint(ref indexPieceToMoveNext, ref directionOfMove);
            Assert.AreEqual(16, indexPieceToMoveNext, "index of piece error");
            Assert.AreEqual(1, directionOfMove, "NSWE index 0123 direction of move error error");

            solver.LoadBoard(new List<int>()
                { 
                    0,0,0,0,1,0,0,//6 - (0-2)
                    0,0,0,0,1,0,0,//13 - (3-5)
                    0,0,1,1,1,0,0,//20 - (6-12)
                    0,0,0,0,1,0,0,//27 - (13-19)
                    0,0,0,0,0,1,0,//34 - (20-26)
                    0,0,0,1,0,0,0,//41 - (27-29)
                    0,0,0,0,0,0,0 //48 -  (30-32)
                },
                LoadState.Current);
            solver.Solve();
            solver.GetHint(ref indexPieceToMoveNext, ref directionOfMove);
            Assert.AreEqual(9, indexPieceToMoveNext, "index of piece error");
            Assert.AreEqual(3, directionOfMove, "NSWE index 0123 direction of move error error");

            solver.LoadBoard(new List<int>()
                { 
                    0,0,0,0,1,0,0,//6 - (0-2)
                    0,0,0,0,1,0,0,//13 - (3-5)
                    0,0,1,0,0,1,0,//20 - (6-12)
                    0,0,0,0,1,0,0,//27 - (13-19)
                    0,0,0,0,0,1,0,//34 - (20-26)
                    0,0,0,1,0,0,0,//41 - (27-29)
                    0,0,0,0,0,0,0 //48 -  (30-32)
                },
                LoadState.Current);
            solver.Solve();
            solver.GetHint(ref indexPieceToMoveNext, ref directionOfMove);
            Assert.AreEqual(2, indexPieceToMoveNext, "index of piece error");
            Assert.AreEqual(1, directionOfMove, "NSWE index 0123 direction of move error error");

            solver.LoadBoard(new List<int>()
                { 
                    0,0,0,0,0,0,0,//6 - (0-2)
                    0,0,0,0,0,0,0,//13 - (3-5)
                    0,0,1,0,1,1,0,//20 - (6-12)
                    0,0,0,0,1,0,0,//27 - (13-19)
                    0,0,0,0,0,1,0,//34 - (20-26)
                    0,0,0,1,0,0,0,//41 - (27-29)
                    0,0,0,0,0,0,0 //48 -  (30-32)
                },
                LoadState.Current);
            solver.Solve();
            solver.GetHint(ref indexPieceToMoveNext, ref directionOfMove);
            Assert.AreEqual(11, indexPieceToMoveNext, "index of piece error");
            Assert.AreEqual(2, directionOfMove, "NSWE index 0123 direction of move error error");

            solver.LoadBoard(new List<int>()
                { 
                    0,0,0,0,0,0,0,//6 - (0-2)
                    0,0,0,0,0,0,0,//13 - (3-5)
                    0,0,1,1,0,0,0,//20 - (6-12)
                    0,0,0,0,1,0,0,//27 - (13-19)
                    0,0,0,0,0,1,0,//34 - (20-26)
                    0,0,0,1,0,0,0,//41 - (27-29)
                    0,0,0,0,0,0,0 //48 -  (30-32)
                },
                LoadState.Current);
            solver.Solve();
            solver.GetHint(ref indexPieceToMoveNext, ref directionOfMove);
            Assert.AreEqual(8, indexPieceToMoveNext, "index of piece error");
            Assert.AreEqual(3, directionOfMove, "NSWE index 0123 direction of move error error");

            solver.LoadBoard(new List<int>()
                { 
                    0,0,0,0,0,0,0,//6 - (0-2)
                    0,0,0,0,0,0,0,//13 - (3-5)
                    0,0,0,0,1,0,0,//20 - (6-12)
                    0,0,0,0,1,0,0,//27 - (13-19)
                    0,0,0,0,0,1,0,//34 - (20-26)
                    0,0,0,1,0,0,0,//41 - (27-29)
                    0,0,0,0,0,0,0 //48 -  (30-32)
                },
                LoadState.Current);
            solver.Solve();
            solver.GetHint(ref indexPieceToMoveNext, ref directionOfMove);
            Assert.AreEqual(10, indexPieceToMoveNext, "index of piece error");
            Assert.AreEqual(1, directionOfMove, "NSWE index 0123 direction of move error error");

            solver.LoadBoard(new List<int>()
                { 
                    0,0,0,0,0,0,0,//6 - (0-2)
                    0,0,0,0,0,0,0,//13 - (3-5)
                    0,0,0,0,0,0,0,//20 - (6-12)
                    0,0,0,0,0,0,0,//27 - (13-19)
                    0,0,0,0,1,1,0,//34 - (20-26)
                    0,0,0,1,0,0,0,//41 - (27-29)
                    0,0,0,0,0,0,0 //48 -  (30-32)
                },
                LoadState.Current);
            solver.Solve();
            solver.GetHint(ref indexPieceToMoveNext, ref directionOfMove);
            Assert.AreEqual(25, indexPieceToMoveNext, "index of piece error");
            Assert.AreEqual(2, directionOfMove, "NSWE index 0123 direction of move error error");

            solver.LoadBoard(new List<int>()
                { 
                    0,0,0,0,0,0,0,//6 - (0-2)
                    0,0,0,0,0,0,0,//13 - (3-5)
                    0,0,0,0,0,0,0,//20 - (6-12)
                    0,0,0,0,0,0,0,//27 - (13-19)
                    0,0,0,1,0,0,0,//34 - (20-26)
                    0,0,0,1,0,0,0,//41 - (27-29)
                    0,0,0,0,0,0,0 //48 -  (30-32)
                },
                LoadState.Current);
            solver.Solve();

            ulong actualBoard=solver.NextBoard;
            solver.GetHint(ref indexPieceToMoveNext, ref directionOfMove);
            Assert.AreEqual(28, indexPieceToMoveNext, "index of piece error");
            Assert.AreEqual(0, directionOfMove, "NSWE index 0123 direction of move error error");

            solver.LoadBoard(65536, LoadState.Win);
            ulong expectedBoard = solver.WinBoard;
            Assert.AreEqual(true,solver.IsSolution, "IsSolution expectation in error");
            Assert.AreEqual(expectedBoard, actualBoard, "win from partial start board in error");
        }

        #region helpers
        
        /// <summary>
        /// Expected bits value accepts strings for significant bit field to change otherwise null
        /// if null uses the defualt empty fields
        /// </summary>
        /// <param name="sbp">SBP</param>
        /// <param name="board">board</param>
        /// <param name="moveID">moveID</param>
        /// <param name="piecesCount">piecesCount</param>
        /// <returns></returns>
        private ulong expectedBitsValue(string board, string moveID, string piecesCount)
        {
            string leadingPacking = "0000 0000 0000 0000";
            string strBoardEmpty = "0000 0000 0000 0000 0000 0000 0000 0000 0";
            string strMoveIDEmpty = "000 000 000";
            string strPiecesCountEmpty = "000 000";

            string strPiecesCount = piecesCount != null ? piecesCount : strPiecesCountEmpty;
            string strMoveID = moveID != null ? moveID : strMoveIDEmpty;


            string strBoard = board != null ? reverseStringOfBits(board) : strBoardEmpty;

            return Convert.ToUInt64(
                    StorageBitPacker.CleanString(
                        leadingPacking +
                        strPiecesCount +
                        strMoveID +
                        strBoard), 2);
        }

        private string reverseStringOfBits(string board)
        {
            char[] chars = StorageBitPacker.CleanString(board).ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }
        
        #endregion //helpers
    }
}
