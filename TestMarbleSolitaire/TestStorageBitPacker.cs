using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using TestMarbleSolitaire.Helpers;
using System.Text;
using System.Diagnostics;
using MarbleSolitaireModelLib.Model;
using MarbleSolitaireLib.GameSolver;
using MarbleSolCommonLib.Common;

namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestStorageBitPacker
    {
        
        //helpers
        Helpers.BitHelpers _helpers = new Helpers.BitHelpers();

        #region setupHelpers
        Helpers.BitHelpers _bitHelpers = new BitHelpers();

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

        MoveController getMovesController()
        {

            return new MoveController(
                new Mapper(getSquareBoard()),
                getSquareBoard(),
                new StorageBitPacker());
        }

        #endregion//setupHelpers


        [TestMethod]
        public void TestSetMasks()
        {
            StorageBitPacker sbp = new StorageBitPacker();
            
            //board mask + move mask + piece mask = 33+9+6  = 48
            //leading packing = 64-48 = 16
            string leadingPacking = "0000 0000 0000 0000";
            
            ulong expectedBoardMask = sbp.ProcessString(leadingPacking+ "000 000" + "000 000 000" + "1111 1111 1111 1111 - 1111 1111 1111 1111 - 1",
                StorageBitPacker.StorageField.All);
            ulong expectedMoveMask = sbp.ProcessString(leadingPacking + "000 000" + "111 111 111" + "0000 0000 0000 0000 - 0000 0000 0000 0000 - 0", 
                StorageBitPacker.StorageField.All);
            ulong expectedPieceMask = sbp.ProcessString(leadingPacking + "111 111" + "000 000 000" + "0000 0000 0000 0000 - 0000 0000 0000 0000 - 0",
                StorageBitPacker.StorageField.All);

            Assert.AreEqual(expectedBoardMask, sbp.BoardMask, "board mask in error");
            Assert.AreEqual(expectedMoveMask, sbp.MoveIDMask, "board mask in error");
            Assert.AreEqual(expectedPieceMask, sbp.PiecesCountMask, "board mask in error");
        }

        [TestMethod]
        public void TestSettingMoveIDs()
        {
            //arrange
            string leadingPacking = "0000 0000 0000 0000";
            string strGameBoardPos = "0000 0000 0000 0000" + "0000 0000 0000 0000" + "0";
            string strPiecesCount = "000 000";

            StorageBitPacker sbp = new StorageBitPacker();

            string strGameBoardPosClean = StorageBitPacker.CleanString(strGameBoardPos);
            string strPiecesCountClean = StorageBitPacker.CleanString(strPiecesCount);

            ulong expectedMoveMask =
                sbp.ProcessString(leadingPacking + strPiecesCount + "111 111 111" + strGameBoardPos, 
                StorageBitPacker.StorageField.All
                );

            ushort[] moveIds = new ushort[] {
                100,300,15,115,215,315,32,232,313,219
            };
            //act
            ulong[] ActualStorageBits = new ulong[] { 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0 
            };

            for (int i = 0; i < moveIds.Length; i++)
            {
                ActualStorageBits[i] = sbp.SetMoveValueID(moveIds[i]);
            }
            //assert
            List<Tuple<int, string>> expectedStorageBitsValueIDs = new List<Tuple<int, string>>()
            {
                new Tuple<int,string>( 1 * 100 + 0,"1100100"),
                new Tuple<int,string>( 3 * 100 + 0,"100101100"),
                new Tuple<int,string>( 0 * 100 + 15,"1111"),
                new Tuple<int,string>( 1 * 100 + 15,"1110011"),
                new Tuple<int,string>( 2 * 100 + 15,"11010111"),
                new Tuple<int,string>( 3 * 100 + 15,"100111011"),
                new Tuple<int,string>( 0 * 100 + 32,"100000"),
                new Tuple<int,string>( 2 * 100 + 32,"11101000"),
                new Tuple<int,string>( 3 * 100 + 13,"100111001"),
                new Tuple<int,string>( 2 * 100 + 19,"11011011"),
            };

            Assert.AreEqual(expectedMoveMask, sbp.MoveIDMask, "assert storageBits move mask error");
            ulong expected = 0;
            int index = 0;
            foreach (var item in expectedStorageBitsValueIDs)
            {
                expected = Convert.ToUInt64(
                    StorageBitPacker.CleanString(
                        leadingPacking +
                        strPiecesCountClean +
                        item.Item2 +
                        strGameBoardPosClean),
                    2);
                Assert.AreEqual(expected, ActualStorageBits[index], 
                    "error with storeageBitsValueID" 
                    + item.Item1.ToString());
                index++;
            }
        }

        [TestMethod]
        public void TestStorageBitsWithFieldSettersAndGetter()
        {
            //arrange
            //string leadingPacking = "0000 0000 0000 0000";
            
            string strBoard = "1 1101 1011 0101 1111" + "0111 0111 1111 1111" ;
            string strMoveID = "001 110 011";
                                
            string strPiecesCount = "011 011";

            StorageBitPacker sbp = new StorageBitPacker();
            ulong storageBits = 
                sbp.ProcessString(
                    strPiecesCount + strMoveID + strBoard, 
                    StorageBitPacker.StorageField.All
                );
            
            sbp.StorageBits = storageBits;
            ulong expected = expectedBitsValue(strBoard, strMoveID, strPiecesCount);
            Assert.AreEqual(expected, sbp.StorageBits, "error setting storage bits");

            expected = expectedBitsValue(strBoard, null, null);
            Assert.AreEqual(expected, sbp.Board, "Getter board after setting storagebits error");

            expected = expectedBitsValue(null, strMoveID, null);
            Assert.AreEqual(expected, sbp.MoveID, "Getter board after setting storagebits error");

            expected = expectedBitsValue(null, null, strPiecesCount);
            Assert.AreEqual(expected, sbp.PiecesCount, "Getter board after setting storagebits error");

            //change values
            string changeMoveID = "011 010 110";
            ushort changeMoveIDasValue = (ushort)sbp.ProcessString(changeMoveID,
                StorageBitPacker.StorageField.MoveID);
            ulong actualMoveID = sbp.SetMoveValueID(changeMoveIDasValue);
            expected = expectedBitsValue(null, changeMoveID, null);
            Assert.AreEqual(expected, actualMoveID, "error setting moveID");
            expected = expectedBitsValue(strBoard, changeMoveID, strPiecesCount);
            Assert.AreEqual(expected, sbp.StorageBits, "error setting moveID in storageBits");

            //set by storagebit fields
            //Set? where ? is board, moveID, or Board respectively
            //change values
            string changePieces = "000 111";
            ushort changePiecesasValue = (ushort)sbp.ProcessString(changePieces,
                StorageBitPacker.StorageField.PiecesCount);
            ulong actualPieces = sbp.SetPiecesCount(changePiecesasValue);
            expected = expectedBitsValue(null, null, changePieces);
            Assert.AreEqual(expected, actualPieces, "error setting actualPieces");
            expected = expectedBitsValue(strBoard, changeMoveID, changePieces);
            Assert.AreEqual(expected, sbp.StorageBits, "error setting moveID in storageBits");

            //change values
            string changeBoard = "1 1101 1000 0000 1111" + "1111 0000 0000 1111"; ;
            ulong changeBoardasValue = sbp.ProcessString(changeBoard,
                StorageBitPacker.StorageField.Board);
            ulong actualBoard = sbp.SetBoard(changeBoardasValue);
            expected = expectedBitsValue(changeBoard, null, null);
            Assert.AreEqual(expected, actualBoard, "error setting actualBoard");
            expected = expectedBitsValue(changeBoard, changeMoveID, changePieces);
            Assert.AreEqual(expected, sbp.StorageBits, "error setting moveID in storageBits");

            //use setter to set packed values
            //change values
            string changeMoveID2 = "111 110 111";
            ulong MoveIDPacked = 4320737099776; //0s + "111 110 111" + 0s;
            sbp.MoveID = MoveIDPacked;
            ulong expectedMoveIDPacked = 4320737099776;
            Assert.AreEqual(expectedMoveIDPacked, sbp.MoveID, "error setting moveID as packed field");
            expected = expectedBitsValue(changeBoard, changeMoveID2, changePieces);
            Assert.AreEqual(expected, sbp.StorageBits, "error setting moveID as packed field in storageBits");

            //change values
            string changePieces2 = "001 100";
            ulong Pieces2Packed = 52776558133248; // "001 100" + 0s;
            sbp.PiecesCount = Pieces2Packed;
            ulong expectedPiecesPacked = 52776558133248;
            Assert.AreEqual(expectedPiecesPacked, sbp.PiecesCount, "error setting PiecesCount as packed field");
            expected = expectedBitsValue(changeBoard, changeMoveID2, changePieces2);
            Assert.AreEqual(expected, sbp.StorageBits, "error setting PiecesCount as packed field in storageBits");

            //change values
            string changeBoard2 = "1 1111 1010 1001 1001" + "1001 1001 1001 1001"; ;
            ulong Board2Packed = 5153960639; // "001 100" + 0s;
            sbp.Board = Board2Packed;
            ulong expectedBoardPacked = 5153960639;
            Assert.AreEqual(expectedBoardPacked, sbp.Board, "error setting board as packed field");
            expected = expectedBitsValue(changeBoard2, changeMoveID2, changePieces2);
            Assert.AreEqual(expected, sbp.StorageBits, "error setting board as packed field in storageBits");
        }

        /// <summary>
        /// Expected bits value accepts strings for significant bit field to change otherwise null
        /// if null uses the defualt empty fields
        /// </summary>
        /// <param name="sbp">SBP</param>
        /// <param name="board">board</param>
        /// <param name="moveID">moveID</param>
        /// <param name="piecesCount">piecesCount</param>
        /// <returns></returns>
        private ulong expectedBitsValue( string board, string moveID, string piecesCount) 
        {
            string leadingPacking = "0000 0000 0000 0000";
            string strBoardEmpty = "0000 0000 0000 0000 0000 0000 0000 0000 0";
            string strMoveIDEmpty = "000 000 000";
            string strPiecesCountEmpty = "000 000";
            
            string strPiecesCount = piecesCount!=null ? piecesCount : strPiecesCountEmpty;
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

        [TestMethod]
        public void TestInitMovesStorageBitsAsValues()
        {
            //arrange
            string leadingPacking = "0000 0000 0000 0000";
            string strGameBoardPos = "0000 0000 0000 0000" + "0000 0000 0000 0000" + "0";
            string strPiecesCount = "000 000";
            string strGameBoardPosClean = StringDropAllButBitChars(strGameBoardPos);
            string strPiecesCountClean = StringDropAllButBitChars(strPiecesCount);

            StorageBitPacker sbp = new StorageBitPacker();
            ulong expectedMoveMask =
                sbp.ProcessString(leadingPacking + strPiecesCount + "111 111 111" + strGameBoardPos,
                StorageBitPacker.StorageField.All
                );

            ICandidates candidates = getSquareBoard();
            SolverBoard solverBoard = new SolverBoard(getLegalPositions());

            //act
            Solver2 solver = new Solver2(candidates);
            MoveController moveController = getMovesController();
            Dictionary<int, MoveSolver2> moves = moveController.Moves;
            display(moves);

            //move mask same each move
            //arbitarily pick centre piece at index 16
            int centreIndexWithEDirectionMove = 3 * 100 + 16;

            //assert
            List<Tuple<int, string>> expectedStorageBitsValueIDs = new List<Tuple<int, string>>()
            {
                new Tuple<int,string>( 1 * 100 + 0,"1100100"),
                new Tuple<int,string>( 3 * 100 + 0,"100101100"),
                new Tuple<int,string>( 0 * 100 + 15,"1111"),
                new Tuple<int,string>( 1 * 100 + 15,"1110011"),
                new Tuple<int,string>( 2 * 100 + 15,"11010111"),
                new Tuple<int,string>( 3 * 100 + 15,"100111011"),
                new Tuple<int,string>( 0 * 100 + 32,"100000"),
                new Tuple<int,string>( 2 * 100 + 32,"11101000"),
                new Tuple<int,string>( 3 * 100 + 13,"100111001"),
                new Tuple<int,string>( 2 * 100 + 19,"11011011"),
            };

            Assert.AreEqual(expectedMoveMask,
                moves[centreIndexWithEDirectionMove].StorageBitsMaskID, "assert storageBits move mask error");
            ulong expected = 0;
            foreach (var item in expectedStorageBitsValueIDs)
            {
                expected = Convert.ToUInt64(
                    StorageBitPacker.CleanString(
                        leadingPacking +
                        strPiecesCountClean +
                        item.Item2 +
                        strGameBoardPosClean),
                    2);
                Assert.AreEqual(expected, moves[item.Item1].StorageBitsValueID, "error with storeageBitsValueID " + item.Item1.ToString());
            }

        }


        [TestMethod]
        public void TestInitPiecesCountValues()
        {
            ushort numberOfpieces = 32;
            PiecesController pc = new PiecesController(numberOfpieces);

            
            Assert.AreEqual((ulong)277076930199552, pc.PiecesMask, "Pieces mask in error");
            Assert.AreEqual((ulong)4398046511104, pc.PiecesStorageBit[0], "Piece count 1 ulong packed value in error");
            Assert.AreEqual((ulong)70368744177664, pc.PiecesStorageBit[15], "Piece count 16 ulong packed value in error");
            Assert.AreEqual((ulong)96757023244288, pc.PiecesStorageBit[21], "Piece count 22 ulong packed value in error");
            Assert.AreEqual((ulong)140737488355328, pc.PiecesStorageBit[31], "Piece count 32 ulong packed value in error");

        }

        [TestMethod]
        public void TestReset()
        {
            StorageBitPacker sbp = new StorageBitPacker();
            sbp.SetPiecesCount(20);
            sbp.SetMoveValueID(117);
            sbp.SetBoard(2481909650);
            
            Assert.IsTrue(sbp.StorageBits != 0 && sbp.MoveID!=0 && sbp.PiecesCount!=0 && sbp.Board!=0, 
                "assert properties have values error ");

            sbp.Reset();
            Assert.IsTrue(sbp.StorageBits == 0 && sbp.MoveID == 0 && sbp.PiecesCount == 0 && sbp.Board == 0,
                "assert properties reset error");
            
        }


        [TestMethod]
        public void TestUpdateAllValuesInStorageBits()
        {
            /*
                    0 1 0       //6 - (0-2)
                    0 1 0       //13 - (3-5)
                0 1 1 1 1 1 0   //20 - (6-12)
                1 1 1 0 1 1 1   //27 - (13-19)
                0 1 1 1 1 1 0   //34 - (20-26)
                    0 1 0       //41 - (27-29)
                    0 1 0       /48 -  (30-32)
            */
            StorageBitPacker sbp = new StorageBitPacker();
            sbp.SetPiecesCount(20);
            sbp.SetMoveValueID(117);
            sbp.SetBoard(2481909650);
            ulong actualStorageBits = sbp.StorageBits;
            
            
            string strPiecesCount = "010 100";
            string strMoveId = "0 0111 0101";
            string strBoard = "0100 1001 1111 0111 0111 0111 1100 1001 0";

            StorageBitPacker expectedSBP = new StorageBitPacker();
            ulong expectedStorageBits =
                expectedSBP.ProcessString(
                    strPiecesCount + strMoveId + strBoard,
                    StorageBitPacker.StorageField.All
                );

            Assert.AreEqual(expectedStorageBits, actualStorageBits, 
                "set Pieces MoveValueID SetBoard error");

            sbp.Reset();
            ulong storageBitsBefore = 0;
            ulong boardPacked = 2481909650; //right most field
            ulong moveIDPacked = 1005022347264;
            ulong piecesCountPacked = 87960930222080;
            ulong updatedStorageBitsValue = sbp.UpdateAll(storageBitsBefore, boardPacked, moveIDPacked, piecesCountPacked);
            sbp.StorageBits = updatedStorageBitsValue;
            actualStorageBits = sbp.StorageBits;
            Assert.AreEqual(expectedStorageBits, actualStorageBits, "storagebits set via updateAll  in error");
        }

        [TestMethod]
        public void TestSettingSequenceStorageBits()
        {
            StorageBitPacker sbp = new StorageBitPacker();
            ulong storageBitsBefore = 0;
            ulong boardPacked = 2481909650; //right most field
            ulong moveIDPacked = 1005022347264;
            ulong piecesCountPacked = 87960930222080;
            ulong updateBitsValue_0 = sbp.UpdateAll(storageBitsBefore, boardPacked, moveIDPacked, piecesCountPacked);
            sbp.StorageBits = updateBitsValue_0;

            /*
                    0 1 0       //6 - (0-2)
                    0 1 0       //13 - (3-5)
                0 1 1 1 1 1 0   //20 - (6-12)
                1 1 1 0 1 1 1   //27 - (13-19)
                0 1 1 1 1 1 0   //34 - (20-26)
                    0 1 0       //41 - (27-29)
                    0 1 0       /48 -  (30-32)
            */
            //make move 14 move east to 16
            ushort piecesCount = 20 - 1;
            ushort moveID = 3 * 100 + 14;
            ulong boardPacked_1 = 2481926034;
            StorageBitPacker sbpHelper = new StorageBitPacker();
            ulong piecesCountPacked_1 = sbpHelper.SetPiecesCount(piecesCount);
            ulong moveIDPacked_1 = sbpHelper.SetMoveValueID(moveID);
            ulong updateBitsValue_1 =
                sbp.UpdateAll(updateBitsValue_0, boardPacked_1, moveIDPacked_1, piecesCountPacked_1);
            sbp.StorageBits = updateBitsValue_1;

            /*
                    0 1 0       //6 - (0-2)
                    0 1 0       //13 - (3-5)
                0 1 1 1 1 1 0   //20 - (6-12)
                1 0 0 1 1 1 1   //27 - (13-19)
                0 1 1 1 1 1 0   //34 - (20-26)
                    0 1 0       //41 - (27-29)
                    0 1 0       /48 -  (30-32)
            */
            //after make move 14 move east to 16
            //make move 8 west to 6
            /* 
                    0 1 0   
                    0 1 0   
                1 0 0 1 1 1 0 
                1 0 0 1 1 1 1 
                0 1 1 1 1 1 0 
                    0 1 0   
                    0 1 0 
             */
            piecesCount = 20 - 1- 1;
            moveID = 2 * 100 + 8;
            ulong boardPacked_2 = 2481925714;
            sbpHelper.Reset();
            ulong piecesCountPacked_2 = sbpHelper.SetPiecesCount(piecesCount);
            ulong moveIDPacked_2 = sbpHelper.SetMoveValueID(moveID);
            ulong updateBitsValue_2 =
                sbp.UpdateAll(updateBitsValue_1, boardPacked_2, moveIDPacked_2, piecesCountPacked_2);
            sbp.StorageBits = updateBitsValue_2;

            //010,010     0,1101,0000     0100,1010,0111,0100,1111,0111,1100,1001,0
            ulong expectedStorageBits = 80954025520722;
            Assert.AreEqual(expectedStorageBits, sbp.StorageBits, "expected values after sequence of updates in error");
        }


        /// <summary>
        /// Converts string 1s and 0s dropping all other characters
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string StringDropAllButBitChars(string s)
        {
            //string s = "110 010 0";
            //convert to sequence of 1s and 0s dropping all else
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (c == '1' || c == '0') sb.Append(c);
            }

            return sb.ToString();
            //Convert.ToUInt64(sb.ToString(),2);

        }

        [Conditional("DEBUG")]
        private void display<T>(IEnumerable<T> value)
        {
            foreach (var item in value)
            {
                Debug.WriteLine(item);
            }
        }
        
    }
}
