//#define LONGRUNNINGON
//#undef LONGRUNNINGON


using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarbleSolitaireLib.Data;
using System.Runtime.Serialization;
using System.Collections.Generic;
using MarbleSolitaireLib.Helpers;
using MarbleSolitaireModelLib.Model;
using TestMarbleSolitaire.Helpers;
using MarbleSolitaireLib.GameSolver;
using System.IO;

namespace TestMarbleSolitaire
{

    [TestClass]
    public class TestEnumSolverIO
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


        #endregion//setupHelpers



        [TestMethod]
        public void TestSaveAndRetrieveFile()
        {
            int piecesCount = 5;
            EnumSolutionsDTO dto = new EnumSolutionsDTO(piecesCount);
            dto.LoadData(1, new List<ulong>() { 1, 2, 3, 4, 5 });
            dto.LoadData(2, new List<ulong>() { 1, 2, 3 });
            dto.LoadData(3, new List<ulong>() { 1, 2 });
            dto.LoadData(4, new List<ulong>() { 1, 2, 3, 4, 5, 6 });
            dto.LoadData(5, new List<ulong>() { 1, 2, 3});

            Assert.IsTrue(dto != null, "error dto should not be null");
            Assert.IsTrue(dto[0][2] == 3, "error expected value dto");
            Assert.IsTrue(dto[2][1] == 2, "error expected value dto");
            Assert.IsTrue(dto[3][0] == 1, "error expected value dto");
            string fileName = "test.Dat";
            SolverIO<IEnumSolutionsDTO>.SaveBinary(dto,fileName);

            EnumSolutionsDTO dto2 = 
                (EnumSolutionsDTO)SolverIO<IEnumSolutionsDTO>.RetrieveBinary(fileName);

            Assert.IsTrue(dto2 != null, "error dto2 should not be null");
            Assert.IsTrue(dto2[0][2] == 3, "error expected value dto2");
            Assert.IsTrue(dto2[2][1] == 2, "error expected value dto2");
            Assert.IsTrue(dto2[3][0] == 1, "error expected value dto2");
        }


        [TestMethod]
        public void TestEnumerateSolnsFromPartialFile()
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
            solver.Solve(SaveState.ToBinary, "EnumDtoPartial.dat");
            
            int piecesCountAtStart = 9;
            EnumSolutionsDTO dto2 =
                (EnumSolutionsDTO)SolverIO<IEnumSolutionsDTO>.RetrieveBinary("EnumDtoPartial.dat");
            //assert
            Assert.IsTrue(dto2 != null, "error dto2 should not be null");
            Assert.AreEqual(piecesCountAtStart,dto2.PiecesCount,"error incorrect number of pieces in file");
            Assert.IsTrue(solver.SolutionCount>1, "expected solution count in error");

        }


#if LONGRUNNINGON

        //[TestMethod, Timeout(3000)]
        //[TestMethod]
        [TestMethod]
        [TestCategory("LongerRuntime")]
        public void TestEnumerateSolnsFromCompleteFile()
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
            solver.Solve(SaveState.ToBinary, "EnumDto.dat");
            int piecesCountAtStart = 32;

            EnumSolutionsDTO dto2 =
                (EnumSolutionsDTO)SolverIO<IEnumSolutionsDTO>.RetrieveBinary();
            //assert
            Assert.IsTrue(dto2 != null, "error dto2 should not be null");
            Assert.AreEqual(piecesCountAtStart, dto2.PiecesCount, "error incorrect number of pieces in file");
            Assert.IsTrue(solver.SolutionCount > 1, "expected solution count in error");

        }



        [TestMethod]
        public void TestSearchBinaryFile()
        {
            string fileNameInTest = "EnumDto.dat";
            bool preCondition = File.Exists(fileNameInTest);
            Assert.IsTrue(preCondition == true, "The file does not exist create it by running TestEnumerateSolnsFromCompleteFile() ");

            EnumSolutionsDTO dto2 =
                (EnumSolutionsDTO)SolverIO<IEnumSolutionsDTO>.RetrieveBinary();

            ulong completedBoardHasSoln = Array.Find(dto2[32-1], x => x == 8589869055);
            Assert.IsTrue(completedBoardHasSoln == 8589869055, "error expected The completed board exists so it has a solution");
            ulong board5Nosoln = Array.Find(dto2[5-1], x => x == 21038336);
            Assert.IsTrue(board5Nosoln == default(ulong), "error expected board5Nosoln has default value therefore no soln");
            ulong board15WithSoln = Array.Find(dto2[15-1], x => x == 1240916416);
            
            
            Assert.IsTrue(board15WithSoln == default(ulong), "error expected Although board15WithSoln {0} has a soln it is not in our list but one of its rotated flipped boards will be", board15WithSoln);

            RotateFlip rf = new RotateFlip(new Mapper(getSquareBoard()));


            board15WithSoln = 1240916416;
            int numberFlipsRotations = 8;
            ulong[] rotatedFlippedBoards = new ulong[numberFlipsRotations];
            rf.GetRotationsFlipsForBoard(board15WithSoln, rotatedFlippedBoards);
            
            
            ulong actualResult=default(ulong);
            for (int i = 0; i < numberFlipsRotations; i++)
            {
                actualResult = Array.Find(dto2[15 - 1], x => x == rotatedFlippedBoards[i]);
                if (actualResult != default(ulong)) break;
            }

            Assert.IsTrue(actualResult != default(ulong), "board15WithSoln expected that we have a soln from the rotated flipped boards is in error");


            ulong board29LongRunningNoSoln = 8589852407;
            rotatedFlippedBoards = new ulong[numberFlipsRotations];
            rf.GetRotationsFlipsForBoard(board29LongRunningNoSoln, rotatedFlippedBoards);


            actualResult = ulong.MaxValue;
            for (int i = 0; i < numberFlipsRotations; i++)
            {
                actualResult = Array.Find(dto2[29 - 1], x => x == rotatedFlippedBoards[i]);
                if (actualResult != default(ulong)) break;
            }

            Assert.IsTrue(actualResult == default(ulong), "board29LongRunningNoSoln expected that we do not have a soln from the rotated flipped boards is in error");
            //ulong board29LongRunningNoSoln = Array.Find(dto2[29-1], x => x == 8589852407);
            //Assert.IsTrue(board29LongRunningNoSoln == default(ulong), "error expected board29LongRunningNoSoln to have no soln");
        }
#endif //LONGRUNNINGON


    }
}
