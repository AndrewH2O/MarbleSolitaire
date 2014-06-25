using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using System.Diagnostics;
using MarbleSolitaireLib.GameSolver;


namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestSolverBoard
    {
        private SolverBoard GetSolverBoard()
        {
            return new SolverBoard(new List<int>() 
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
        public void TestSetMove()
        {
            ulong preMove = 0;
            ulong expectedPostMove = 0;
            ulong move = 0;
            SolverBoard solverBoard = GetSolverBoard();
            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,0,0,0,0,0, //6
                0,0,0,0,0,0,0, //13
                0,0,0,0,0,0,0, //20
                0,1,1,0,0,0,0, //27
                0,0,0,0,0,0,0, //34
                0,0,0,0,0,0,0, //41
                0,0,0,0,0,0,0  //48
            });

            preMove = solverBoard.BoardCurrent;
            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,0,0,0,0,0, //6
                0,0,0,0,0,0,0, //13
                0,0,0,0,0,0,0, //20
                0,0,0,1,0,0,0, //27
                0,0,0,0,0,0,0, //34
                0,0,0,0,0,0,0, //41
                0,0,0,0,0,0,0  //48
            });
            expectedPostMove = solverBoard.BoardCurrent;
            int source = 22;
            int jumped = 23;
            int target = 24;
            solverBoard.ClearBoard();
            solverBoard.SetPostMove(source, jumped, target, ref preMove);
            move = preMove;
            Assert.AreEqual(expectedPostMove, move, "Set move in error");
        }

        [TestMethod]
        public void TestUndoMove()
        {
            ulong expectedMove = 0;
            ulong move = 0;
            SolverBoard solverBoard = GetSolverBoard();
            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,0,0,0,0,0, //6
                0,0,0,0,0,0,0, //13
                0,0,0,0,0,0,0, //20
                0,1,1,0,0,0,0, //27
                0,0,0,0,0,0,0, //34
                0,0,0,0,0,0,0, //41
                0,0,0,0,0,0,0  //48
            });

            expectedMove = solverBoard.BoardCurrent;
            int source = 22;
            int jumped = 23;
            int target = 24;
            solverBoard.ClearBoard();
            solverBoard.SetPreMove(source, jumped, target, ref move);

            Assert.AreEqual(expectedMove, move, "Undo move in error");
        }

        [TestMethod]
        public void TestPremaskPostMoves()
        {
            //arrange
            UInt64 preMove = 0;
            UInt64 maskMove = 0;
            UInt64 postMove = 0;
            //arbitrary move - irrelevant if its a legal move
            int source = 10;
            int jumped = 17;
            int target = 24;

            SolverBoard solverBoard = GetSolverBoard();

            solverBoard.SetPreMove(source, jumped, target, ref preMove);
            solverBoard.SetMaskMove(source, jumped, target, ref maskMove);
            solverBoard.SetPostMove(source, jumped, target, ref postMove);


            double PreExpected = 0;
            PreExpected += Math.Pow(2, source);
            PreExpected += Math.Pow(2, jumped);

            double PostExpected = 0;
            PostExpected += Math.Pow(2, target);

            double MaskExpected = 0;
            MaskExpected += Math.Pow(2, source);
            MaskExpected += Math.Pow(2, jumped);
            MaskExpected += Math.Pow(2, target);

            Assert.AreEqual(MaskExpected, maskMove, "Mask set incorrectly");
            Assert.AreEqual(PreExpected, preMove, "Pre move set incorrectly");
            Assert.AreEqual(PostExpected, postMove, "Post move set incorrectly");
        }


        [TestMethod]
        public void TestIsMovePossibleOnBitBoard()
        {
            //arrange
            bool isMovePossible = false;
            SolverBoard solverBoard = GetSolverBoard();
            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,1,0,0,0, //13
                1,1,1,1,0,0,0, //20
                0,1,1,0,1,1,1, //27
                0,0,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
               
            });

            int source = 10;
            int jumped = 17;
            int target = 24;
            UInt64 preMove = 0;
            UInt64 maskMove = 0;
            UInt64 postMove = 0;

            solverBoard.SetPreMove(source, jumped, target, ref preMove);
            solverBoard.SetMaskMove(source, jumped, target, ref maskMove);
            solverBoard.SetPostMove(source, jumped, target, ref postMove);
            //act 
            isMovePossible = (solverBoard.BoardCurrent & maskMove) == preMove;
            //assert
            Assert.AreEqual(isMovePossible, true, "error in setting move");


        }

        [TestMethod]
        public void TestIsMovePossibleOnBitBoardWithAllBitsClear()
        {
            //arrange
            bool isMovePossible = true;
            SolverBoard solverBoard = GetSolverBoard();
            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,0,0,0,0, //13
                1,1,1,0,0,0,0, //20
                0,1,1,0,1,1,1, //27
                0,0,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
               
            });

            int source = 10;
            int jumped = 17;
            int target = 24;
            UInt64 preMove = 0;
            UInt64 maskMove = 0;
            UInt64 postMove = 0;

            solverBoard.SetPreMove(source, jumped, target, ref preMove);
            solverBoard.SetMaskMove(source, jumped, target, ref maskMove);
            solverBoard.SetPostMove(source, jumped, target, ref postMove);
            //act 
            isMovePossible = (solverBoard.BoardCurrent & maskMove) == preMove;
            //assert
            Assert.AreEqual(isMovePossible, false, "error in setting move with all significant bits clear");


        }

        [TestMethod]
        public void TestIsMovePossibleOnBitBoardWithAllBitsSet()
        {
            //arrange
            bool isMovePossible = true;
            SolverBoard solverBoard = GetSolverBoard();
            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,1,0,0,0, //13
                1,1,1,1,0,0,0, //20
                0,1,1,1,1,1,1, //27
                0,0,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
               
            });

            int source = 10;
            int jumped = 17;
            int target = 24;
            UInt64 preMove = 0;
            UInt64 maskMove = 0;
            UInt64 postMove = 0;

            solverBoard.SetPreMove(source, jumped, target, ref preMove);
            solverBoard.SetMaskMove(source, jumped, target, ref maskMove);
            solverBoard.SetPostMove(source, jumped, target, ref postMove);
            //act 
            isMovePossible = (solverBoard.BoardCurrent & maskMove) == preMove;
            //assert
            Assert.AreEqual(isMovePossible, false, "error in setting move with all significant bits set");


        }

        [TestMethod]
        public void TestIsMovePossibleOnBitBoardWithIncorrectPreSetup()
        {
            //arrange
            bool isMovePossible = true;
            SolverBoard solverBoard = GetSolverBoard();
            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,0,0,0,0, //13
                1,1,1,1,0,0,0, //20
                0,1,1,0,1,1,1, //27
                0,0,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
               
            });

            int source = 10;
            int jumped = 17;
            int target = 24;
            UInt64 preMove = 0;
            UInt64 maskMove = 0;
            UInt64 postMove = 0;

            solverBoard.SetPreMove(source, jumped, target, ref preMove);
            solverBoard.SetMaskMove(source, jumped, target, ref maskMove);
            solverBoard.SetPostMove(source, jumped, target, ref postMove);
            //act 
            isMovePossible = (solverBoard.BoardCurrent & maskMove) == preMove;
            //assert
            Assert.AreEqual(isMovePossible, false, "error in setting move with incorrect pre position");


        }

        [TestMethod]
        public void TestMakeMoveOnBitBoard()
        {
            //arrange
            SolverBoard solverBoard = GetSolverBoard();
            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,1,0,0,0, //13
                1,1,1,1,0,0,0, //20
                0,1,1,0,1,1,1, //27
                0,0,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
               
            });

            int source = 10;
            int jumped = 17;
            int target = 24;
            UInt64 preMove = 0;
            UInt64 maskMove = 0;
            UInt64 postMove = 0;

            solverBoard.SetPreMove(source, jumped, target, ref preMove);
            solverBoard.SetMaskMove(source, jumped, target, ref maskMove);
            solverBoard.SetPostMove(source, jumped, target, ref postMove);
            //act 
            ulong postMoveBits = (solverBoard.BoardCurrent & ~maskMove) | postMove;

            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,0,0,0,0, //13
                1,1,1,0,0,0,0, //20
                0,1,1,1,1,1,1, //27
                0,0,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
               
            });

            ulong expectedMoveHasBeenMade = solverBoard.BoardCurrent;
            //assert
            Assert.AreEqual(expectedMoveHasBeenMade, postMoveBits, "error in making move");

            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,1,0,0,0, //13
                1,1,1,0,0,0,0, //20
                0,1,1,1,1,1,1, //27
                0,0,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
               
            });
            //act
            //001
            postMoveBits = (solverBoard.BoardCurrent & ~maskMove) | postMove;
            ulong expectedMoveHasNotBeenMade = solverBoard.BoardCurrent;
            Debug.WriteLine(solverBoard.DisplayBoard(postMoveBits, "makeMove", 7));
            //assert
            Assert.AreNotEqual(expectedMoveHasNotBeenMade, postMoveBits, "error in making move from incorrect position");

        }

        [TestMethod]
        public void TestUndoMoveOnBitBoard()
        {
            //arrange
            SolverBoard solverBoard = GetSolverBoard();
            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,0,0,0,0, //13
                1,1,1,0,0,0,0, //20
                0,1,1,1,1,1,1, //27
                0,0,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
               
            });

            int source = 10;
            int jumped = 17;
            int target = 24;
            UInt64 preMove = 0;
            UInt64 maskMove = 0;
            UInt64 postMove = 0;

            solverBoard.SetPreMove(source, jumped, target, ref preMove);
            solverBoard.SetMaskMove(source, jumped, target, ref maskMove);
            solverBoard.SetPostMove(source, jumped, target, ref postMove);
            //act 
            //110
            ulong undoMoveBits = (solverBoard.BoardCurrent & ~maskMove) | preMove;
            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,1,0,0,0, //13
                1,1,1,1,0,0,0, //20
                0,1,1,0,1,1,1, //27
                0,0,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
               
            });
            ulong expectedUndoMoveOnBoard = solverBoard.BoardCurrent;
            //assert
            Assert.AreEqual(expectedUndoMoveOnBoard, undoMoveBits, "error in undoing move");

            solverBoard.SetCurrentPositions(new List<int>() 
            { 
                0,0,1,1,0,0,0, //6
                0,0,1,1,0,0,0, //13
                1,1,1,0,0,0,0, //20
                0,1,1,1,1,1,1, //27
                0,0,1,1,1,1,1, //34
                0,0,1,1,1,0,0, //41
                0,0,1,1,1,0,0  //48
               
            });
            //101
            undoMoveBits = (solverBoard.BoardCurrent & ~maskMove) | preMove;
            ulong expectedUndoMoveIncorrect = solverBoard.BoardCurrent;
            Debug.WriteLine(solverBoard.DisplayBoard(undoMoveBits, "undo", 7));
            //assert
            Assert.AreNotEqual(expectedUndoMoveIncorrect, undoMoveBits, "error in undoing move from incorrect position");
        }



        
    }
}
