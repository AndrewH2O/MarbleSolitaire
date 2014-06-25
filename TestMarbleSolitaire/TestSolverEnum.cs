//#define LONGRUNNINGON
//#undef LONGRUNNINGON

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MarbleSolitaireModelLib.Model;
using MarbleSolitaireLib.GameSolver;
using TestMarbleSolitaire.Helpers;
using System.Diagnostics;

namespace TestMarbleSolitaire
{
#if LONGRUNNINGON
    [TestClass]
    public class TestSolverEnum
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

        const int TOTAL_SOLN_COUNT_AFTER_ONE_MOVE = 1679071;
        const int TOTAL_SOLN_COUNT = 1679072;



        #endregion//setupHelpers
        
        
        [TestMethod]
        public void TestEnumerateSolnsFromPartialBoard()
        {
            //arrange
            SolverEnum solver = new SolverEnum(getSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,0,0,1,0,0,//6 - (0-2)
                0,0,0,0,1,0,0,//13 - (3-5)
                0,0,1,1,1,0,0,//20 - (6-12)
                0,0,0,1,1,0,0,//27 - (13-19)
                0,0,0,1,0,1,0,//34 - (20-26)
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
            //act
            solver.Solve();
            
            //assert
            Assert.IsTrue(solver.SolutionCount > 1, "expected solution count in error");
            
         
        }

        //[TestMethod, Timeout(3000)]
        //[TestMethod, Timeout(5000)]
        
        //[TestMethod, Timeout(5000)]
        [TestMethod]
        [TestCategory("LongerRuntime")]
        public void TestEnumerateSolnsFromCompleteBoard()
        {
            //arrange
            SolverEnum solver = new SolverEnum(getSquareBoard());

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
            //act

            //solver.Solve(SaveState.ToText);
            solver.Solve();
            
            //assert
            Assert.IsTrue(solver.SolutionCount == TOTAL_SOLN_COUNT, "expected solution count in error");


        }


        //[TestMethod, Timeout(3000)]
        //[TestMethod]
        [TestMethod]
        [TestCategory("LongerRuntime")]
        public void TestEnumerateSolnsAfterMove1Rotated0()
        {
            //arrange
            SolverEnum solver = new SolverEnum(getSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,0,1,0,0,//13 - (3-5)
                1,1,1,0,1,1,1,//20 - (6-12)
                1,1,1,1,1,1,1,//27 - (13-19)
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
            //act


            solver.Solve();

            //assert
            Assert.IsTrue(solver.SolutionCount == TOTAL_SOLN_COUNT_AFTER_ONE_MOVE, "expected solution count in error");


        }

        //[TestMethod, Timeout(3000)]
        //[TestMethod]
        [TestMethod]
        [TestCategory("LongerRuntime")]
        public void TestEnumerateSolnsAfterMove1Rotated90()
        {
            //arrange
            SolverEnum solver = new SolverEnum(getSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,1,0,0,1,//27 - (13-19)
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
            //act


            solver.Solve();

            //assert
            Assert.IsTrue(solver.SolutionCount == TOTAL_SOLN_COUNT_AFTER_ONE_MOVE, "expected solution count in error");


        }

        //[TestMethod, Timeout(3000)]
        //[TestMethod]
        [TestMethod]
        [TestCategory("LongerRuntime")]
        public void TestEnumerateSolnsAfterMove1Rotated180()
        {
            //arrange
            SolverEnum solver = new SolverEnum(getSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,1,1,1,1,//27 - (13-19)
                1,1,1,0,1,1,1,//34 - (20-26)
                0,0,1,0,1,0,0,//41 - (27-29)
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
            //act


            solver.Solve();

            //assert
            Assert.IsTrue(solver.SolutionCount == TOTAL_SOLN_COUNT_AFTER_ONE_MOVE, "expected solution count in error");


        }

        //[TestMethod, Timeout(3000)]
        //[TestMethod]
        [TestMethod]
        [TestCategory("LongerRuntime")]
        public void TestEnumerateSolnsAfterMove1Rotated270()
        {
            //arrange
            SolverEnum solver = new SolverEnum(getSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,0,0,1,1,1,1,//27 - (13-19)
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
            //act


            solver.Solve();

            //assert
            Assert.IsTrue(solver.SolutionCount == TOTAL_SOLN_COUNT_AFTER_ONE_MOVE, "expected solution count in error");


        }

        //[TestMethod, Timeout(3000)]
        //[TestMethod]
        [TestMethod]
        [TestCategory("LongerRuntime")]
        public void TestEnumerateSolnsWhereUnknownSoln()
        {
            //arrange
            SolverEnum solver = new SolverEnum(getSquareBoard());

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
            //act


            solver.Solve();

            //assert
            Assert.IsTrue(solver.SolutionCount == 0, "expected solution count in error");


        }
    }
#endif //LONGRUNNINGON
}
