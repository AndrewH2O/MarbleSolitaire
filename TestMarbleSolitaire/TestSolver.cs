using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using TestMarbleSolitaire.Helpers;
using System.Diagnostics;
using MarbleSolitaireModelLib.Model;
using MarbleSolitaireLib.GameSolver;
using MarbleSolCommonLib.Common;


namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestSolver
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
            SquareBoard sqb= new SquareBoard(
                getLegalPositions(), new FakeErrorLog());
            
            sqb.SetupStart(getStart());

            return sqb;
        }
        #endregion//setupHelpers


        [TestMethod]
        public void TestCachingAllMoves()
        {
            //arrange
            Solver solver = new Solver(new SolverBoard(getLegalPositions()));

            ICandidates candidates = getSquareBoard();
            //act
            solver.InitialiseMoves(candidates);

            int expectedCountOfNodes=(4*6)+9;
            //assert
            Assert.AreEqual(
                solver.Nodes.Count, 
                expectedCountOfNodes, "solver node count error");
        }

        [TestMethod]
        public void TestMoveCachedOk()
        {
            //arrange
            Solver solver = new Solver(new SolverBoard(getLegalPositions()));

            ICandidates candidates = getSquareBoard();
            //act
            solver.InitialiseMoves(candidates);
            //assert
            AssertNSWEMovesForNode(0, solver.Nodes[0], new bool[] { false, true, false, true });
            AssertNSWEMovesForNode(32, solver.Nodes[32], new bool[] { true, false, true, false });
            AssertNSWEMovesForNode(16, solver.Nodes[16], new bool[] { true, true, true, true });
            AssertNSWEMovesForNode(14, solver.Nodes[14], new bool[] { false,false, false, true });
            AssertNSWEMovesForNode(25, solver.Nodes[25], new bool[] { true, false, true, false });
            AssertNSWEMovesForNode(8, solver.Nodes[8], new bool[] { true, true, true, true });
            AssertNSWEMovesForNode(23, solver.Nodes[23], new bool[] { true, true, true, true });
            AssertNSWEMovesForNode(19, solver.Nodes[19], new bool[] { false, false, true, false });
        }

        private void AssertNSWEMovesForNode(int index,SolverNode solverNode, bool[] expected)
        {
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(solverNode.Mask[i] != 0, expected[i], "error in mask "+index);
                Assert.AreEqual(solverNode.PostMoves[i] != 0, expected[i], "error in postMoves" + index);
                Assert.AreEqual(solverNode.PreMoves[i] != 0, expected[i], "error in preMoves" + index);
            }
        }
        
        [TestMethod]
        public void TestGetMoves()
        {
            //arrange
            Solver solver = new Solver(new SolverBoard(getLegalPositions()));
            ICandidates candidates = getSquareBoard();
            solver.InitialiseMoves(candidates);
            solver.LoadStart(new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            });
            ulong currentBoard = solver.CurrentBoard;
            
            Stack<SolverMove> moves = new Stack<SolverMove>();
            //act
            solver.FindAvailableMoves(currentBoard);
            moves = solver.AvailableMoves;
            int expectedNumberMoves = 4;           
            //assert
            Assert.AreEqual(moves.Count, expectedNumberMoves, "Incorrect available move count");
            SolverMove move = moves.Pop();
            AssertMoves(move.Index, getDirection(move.Direction), 28, NSWE.N);
            move = moves.Pop();
            AssertMoves(move.Index, getDirection(move.Direction), 18, NSWE.W);
            move = moves.Pop();
            AssertMoves(move.Index, getDirection(move.Direction), 14, NSWE.E);
            move = moves.Pop();
            AssertMoves(move.Index, getDirection(move.Direction), 4, NSWE.S);
            Assert.IsTrue(moves.Count == 0, "error in checking moves");
             
        }

        #region helperMethods
        private void AssertMoves(int actualIndex, NSWE actualDirection, int expectedIndex, NSWE expectedDirection)
        {
            Assert.AreEqual(actualIndex, expectedIndex, "move available index incorrect");
            Assert.AreEqual(actualDirection, expectedDirection, "move available direction incorrect");
        }

        private NSWE getDirection(int move)
        {
            switch (move)
            {
                case 0:
                    return NSWE.N;
                case 1:
                    return NSWE.S;
                case 2:
                    return NSWE.W;
                case 3:
                    return NSWE.E;
                default:
                    break;
            }

            return NSWE.Undefined;
        }

        enum NSWE { N = 0, S = 1, W = 2, E = 3, Undefined = 4 }
        #endregion//helperMethods


        [TestMethod]
        public void TestMake5Moves()
        {
            //arrange
            Solver solver = new Solver(new SolverBoard(getLegalPositions()));
            ICandidates candidates = getSquareBoard();
            solver.InitialiseMoves(candidates);
            solver.LoadStart(new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            });
            ulong startBoard = solver.CurrentBoard;
            Debug.WriteLine("Start: \n"+solver.DisplayCurrentBoard());
            Stack<int> moves = new Stack<int>();
            //act
            //solver.Solve(5,startBoard);
            solver.Solve(5);
            ulong actualNextBoardAfterMoves = solver.NextBoard;
            ulong currentBoardAfter5Moves = solver.CurrentBoard;
            Debug.WriteLine("Actual after 5 moves: \n"+solver.DisplayCurrentBoard());
            //arrange: use solver to convert board to ulong
            solver.LoadStart(new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,1,1,1,1,//27 - (13-19)
                1,1,1,0,0,1,1,//34 - (20-26)
                0,0,1,0,0,0,0,//41 - (27-29)
                0,0,0,0,1,0,0//48 -  (30-32)
            });
            
            ulong expectedBoard = solver.CurrentBoard;
            Assert.AreEqual(expectedBoard, actualNextBoardAfterMoves, "After 5 Moves");
            
            Assert.AreEqual(startBoard, currentBoardAfter5Moves, "expected that CurrentBoard should be unchanged");
        }

        [TestMethod]
        public void TestMake15Moves()
        {
            //arrange
            Solver solver = new Solver(new SolverBoard(getLegalPositions()));
            ICandidates candidates = getSquareBoard();
            solver.InitialiseMoves(candidates);
            solver.LoadStart(new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            });
            ulong currentBoard = solver.CurrentBoard;

            Stack<int> moves = new Stack<int>();
            //act
            //solver.Solve(15, currentBoard);
            solver.Solve(15);
            ulong actualNextBoardAfterMoves = solver.NextBoard;
            //arrange: use solver to convert board to ulong
            solver.LoadStart(new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                0,0,0,0,1,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,1,0,1,0,0,//41 - (27-29)
                0,0,1,0,0,0,0//48 -  (30-32)
            });
            ulong expectedBoard = solver.CurrentBoard;
            Assert.AreEqual(expectedBoard, actualNextBoardAfterMoves, "After 15 Moves");
        }

        [TestMethod]
        public void TestMake26Moves()
        {
            //descended to lowest depth ordering highest index first and where
            //there is a choice in direction of move chooses EWSN
            
            //arrange
            Solver solver = new Solver(new SolverBoard(getLegalPositions()));
            ICandidates candidates = getSquareBoard();
            solver.InitialiseMoves(candidates);
            solver.LoadStart(new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            });
            ulong currentBoard = solver.CurrentBoard;

            Stack<int> moves = new Stack<int>();
            //act
            //solver.Solve(26, currentBoard);
            solver.Solve(26);
            ulong actualNextBoardAfterMoves = solver.NextBoard;
            //arrange: use solver to convert board to ulong
            solver.LoadStart(new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,1,0,0,0,0,//20 - (6-12)
                0,0,0,0,1,0,0,//27 - (13-19)
                0,0,1,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0//48 -  (30-32)
            });
            
            ulong expectedBoard = solver.CurrentBoard;
            //assert
            Assert.AreEqual(expectedBoard, actualNextBoardAfterMoves, "After 26 Moves");
        }

        [TestMethod]
        public void TestFindSolutionPartial()
        {
            //descended to lowest depth ordering highest index first and where
            //there is a choice in direction of move chooses EWSN

            //arrange
            Solver solver = new Solver(new SolverBoard(getLegalPositions()));
            ICandidates candidates = getSquareBoard();
            solver.InitialiseMoves(candidates);
            solver.LoadStart(new List<int>() 
            { 
                0,0,0,0,1,0,0,//6 - (0-2)
                0,0,0,0,1,0,0,//13 - (3-5)
                0,0,1,1,1,0,0,//20 - (6-12)
                0,0,0,1,1,0,0,//27 - (13-19)
                0,1,1,1,1,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            });

            solver.SetWinningState(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            });
            
            ulong currentBoard = solver.CurrentBoard;

            Stack<int> moves = new Stack<int>();
            //act
            //solver.Solve(-1, currentBoard);
            solver.Solve();
            bool expectedSolution = true;

            Assert.AreEqual(expectedSolution, solver.IsSolution, 
                "Failed to find the expected solution");
            ulong expectedSolutionBoard = solver.GetSolutionPart(0);
            //arrange: use solver to convert board to ulong
            solver.LoadStart(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            });

            Assert.AreEqual(expectedSolutionBoard, solver.CurrentBoard, 
                "Expected solution state of board error");
        }

        [TestMethod]
        public void TestFindSolutionComplete()
        {
            //descended to lowest depth ordering highest index first and where
            //there is a choice in direction of move chooses EWSN

            //arrange
            Solver solver = new Solver(new SolverBoard(getLegalPositions()));
            ICandidates candidates = getSquareBoard();
            solver.InitialiseMoves(candidates);
            solver.LoadStart(new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            });
            
            solver.SetWinningState(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            });
            
            ulong currentBoard = solver.CurrentBoard;

            Stack<int> moves = new Stack<int>();
            //act
            //solver.Solve(-1, currentBoard);
            solver.Solve();
            bool expectedSolution = true;

            Assert.AreEqual(expectedSolution, solver.IsSolution,
                "Failed to find the expected solution");
            ulong expectedSolutionBoard = solver.GetSolutionPart(0);
            //arrange: use solver to convert board to ulong
            solver.LoadStart(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            });

            Assert.AreEqual(expectedSolutionBoard, solver.CurrentBoard,
                "Expected solution state of board error");
        }

        [TestMethod]
        public void TestNoSolutionFound()
        {
            //descended to lowest depth ordering highest index first and where
            //there is a choice in direction of move chooses EWSN

            //arrange
            Solver solver = new Solver(new SolverBoard(getLegalPositions()));
            ICandidates candidates = getSquareBoard();
            solver.InitialiseMoves(candidates);
            solver.LoadStart(new List<int>() 
            { 
                0,0,1,0,0,0,0,//6 - (0-2)
                0,0,1,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,1,1,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                1,1,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,1,0,0,//41 - (27-29)
                0,0,0,0,1,0,0//48 -  (30-32)
            });

            solver.SetWinningState(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            });

            ulong currentBoard = solver.CurrentBoard;

            Stack<int> moves = new Stack<int>();
            //act
            //solver.Solve(-1, currentBoard);
            solver.Solve();
            bool expectedSolution = false;

            Assert.AreEqual(expectedSolution, solver.IsSolution,
                "Failed to find that there is no solution");
            //arrange: use solver to convert board to ulong
            solver.LoadStart(new List<int>() 
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,1,0,1,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,1,0,1,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            });
            ulong expectedFinalBoard = solver.CurrentBoard;
            Assert.AreEqual(expectedFinalBoard, solver.GetBestBoardWhereNoSolution(),
                "Expected no solution state of board error");
        }
    }

}
