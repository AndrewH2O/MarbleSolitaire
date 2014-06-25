

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarbleSolitaireLib.GameSolver;
using MarbleSolitaireLib.Data;
using System.Collections.Generic;
using MarbleSolitaireModelLib.Model;
using TestMarbleSolitaire.Helpers;
using System.Diagnostics;

namespace TestMarbleSolitaire
{
    
    [TestClass]
    public class TestEnumSolnsData
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
            return new RotateFlip(new Mapper(getSquareBoard()));
        }

        EnumSolutionsDTO getEnumSolutionsDTO()
        {
            bool isError = false;
            EnumSolutionsDTO dto = DataLoader.GetData(out isError);
            Assert.IsFalse(isError, "data loaded ok");
            return dto;
        }

        #endregion//setupHelpers
        
        
        [TestMethod]
        public void TestSearchForASolutionByBoardAndPieces
            ()
        {
            EnumSolutionsDTO dto = getEnumSolutionsDTO();
            RotateFlip rf = getRotateFlip();
            EnumSolutionsData es = new EnumSolutionsData(dto, rf);

            ulong completedBoardHasSoln = 8589869055;
            ulong board5Nosoln = 21038336;
            ulong board15WithSoln = 1240916416;
            ulong board29LongRunningNoSoln = 8589852407;
            
            Assert.AreEqual(true, es.IsSolution(completedBoardHasSoln,32), "error expected solution");
            Assert.AreEqual(false, es.IsSolution(board5Nosoln, 5), "error expected no soln");
            Assert.AreEqual(true, es.IsSolution(board15WithSoln,15), "error expected a soln");
            Assert.AreEqual(false, es.IsSolution(board29LongRunningNoSoln,29), "error expected no soln");
        }

        [TestMethod]
        public void TestSearchForASolutionByBoard()
        {
            EnumSolutionsDTO dto = getEnumSolutionsDTO();
            RotateFlip rf = getRotateFlip();

            EnumSolutionsData es = new EnumSolutionsData(dto, rf);

            ulong completedBoardHasSoln = 8589869055;
            ulong board5Nosoln = 21038336;
            ulong board15WithSoln = 1240916416;
            ulong board29LongRunningNoSoln = 8589852407;

            Assert.AreEqual(true, es.IsSolution(completedBoardHasSoln), "error expected solution");
            Assert.AreEqual(false, es.IsSolution(board5Nosoln), "error expected no soln");
            Assert.AreEqual(true, es.IsSolution(board15WithSoln), "error expected a soln");
            Assert.AreEqual(false, es.IsSolution(board29LongRunningNoSoln), "error expected no soln");
        }

        

        [TestMethod]
        public void TestCountUniqueSolutions()
        {
            EnumSolutionsDTO dto = getEnumSolutionsDTO();
            RotateFlip rf = getRotateFlip();

            EnumSolutionsData es = new EnumSolutionsData(dto, rf);
            int count = es.GetCountOfUniqueSolutions();
            Debug.Write(es.DisplayUniqueBoardsCountsByPiece());
            Assert.AreEqual(1679072, count, "count error");
        }

        [TestMethod]
        public void TestForSymmetryInSolnCountsByPieces()
        {
            EnumSolutionsDTO dto = getEnumSolutionsDTO();
            RotateFlip rf = getRotateFlip();

            EnumSolutionsData es = new EnumSolutionsData(dto, rf);
            ulong maxPieces = 32;
            bool isSymmetrical = false;
            for (ulong i = 0; i < maxPieces/2 && i < maxPieces  ; i++)
            {
                //isSymmetrical = es[i].Count == es[maxPieces - 1 - i].Count;
                isSymmetrical = es[i] == es[maxPieces - 1 - i];//es indexing is an array containing count
                if (!isSymmetrical) break;
            }
            Assert.IsTrue(isSymmetrical, "error with symmetry");
        }

        [TestMethod]
        public void TestTotalDistinctBoardsByPieceNumber()
        {
            EnumSolutionsDTO dto = getEnumSolutionsDTO();
            RotateFlip rf = getRotateFlip();

            EnumSolutionsData es = new EnumSolutionsData(dto, rf);
            int count = es.GetCountTotalDistinctSolutions();
            //int expected = 13428122;
            int uniqueCountBeforeRF = 1679072;
            int numberRFs =8;
            Debug.Write(es.DisplayTotalDistinctBoardsCountsByPiece());
            //symmetry eg board with 1 piece after rotations and flips have 8 all the same
            //total must be less than 8 * uniqueCountBeforeRFi
            Assert.IsTrue(count < numberRFs * uniqueCountBeforeRF, "error TotalDistinctBoards count");
        }
    }


}