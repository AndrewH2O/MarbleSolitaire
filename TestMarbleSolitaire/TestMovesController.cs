#define SOURCE_HAS_TEST_DEF
//#undef SOURCE_HAS_TEST_DEF

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using TestMarbleSolitaire.Helpers;

using System.Diagnostics;
using System.Text;
using MarbleSolitaireModelLib.Model;
using MarbleSolitaireLib.GameSolver;
using MarbleSolCommonLib.Common;

namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestMovesController
    {
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
        public void TestBuildMoves()
        {
            
            uint indexMove = 0;
            ICandidates candidates = getSquareBoard();

            int NON_LEGAL = candidates.TokenIllegalPosition;
            int numberOfMoves = 76;
            Dictionary<int, int> mapIndexModelToGame = getMapIndexToGame();
            Dictionary<int, Solver2Move1> moves = new Dictionary<int, Solver2Move1>(numberOfMoves);

            foreach (var indexModel in mapIndexModelToGame.Keys)
            {
                buildMoves(
                    candidates.GetListOfSourceCandidates(indexModel),
                    candidates.GetListOfJumpedCandidates(indexModel),
                    candidates.GetListOfTargetCandidates(indexModel),
                    indexModel,
                    ref indexMove,
                    moves,
                    mapIndexModelToGame
                );
            }

            Assert.AreEqual(moves.Count, 76, "Error in number of moves");

            //access moves nswe direction (values 0-3) * 100 + index into game board (0-32)

            int[][] ids = new int[][]{
                new int[] { 1 * 100 + 0, 0},//moveID, moveIndex
                new int[] { 3 * 100 + 0,1},
                new int[] { 0 * 100 + 15,32},
                new int[] { 1 * 100 + 15,33},
                new int[] { 2 * 100 + 15,34},
                new int[] { 3 * 100 + 15,35},
                new int[] { 0 * 100 + 32,74},
                new int[] { 2 * 100 + 32,75},
                new int[] { 3 * 100 + 13,30},
                new int[] { 2 * 100 + 19,45}
            };
            for (int i = 0; i < ids.Length; i++)
            {
                Assert.IsTrue(moves[ids[i][0]] != null, "error move exists " + ids[i][0].ToString());
                Assert.AreEqual((ushort)(ids[i][0]), moves[ids[i][0]].ID, "error id value " + ids[i].ToString());
                Assert.AreEqual((ushort)(ids[i][0] % 100), moves[ids[i][0]].IndexGame, "error indexGame value " + ids[i].ToString());
                Assert.AreEqual((byte)((ids[i][0] - (ids[i][0] % 100)) / 100), moves[ids[i][0]].Direction, "error nswe direction " + ids[i].ToString());
                Assert.AreEqual(ids[i][1], moves[ids[i][0]].IndexMove, "error indexMove value " + ids[i].ToString());
            }
        }

        private Dictionary<int, int> getMapIndexToGame()
        {
            

            //Dictionary<int, int> _mapIndexGameToModel = new Dictionary<int, int>(numberGamePos);
            Dictionary<int, int> mapIndexModelToGame = new Dictionary<int, int>();
            int indexInGame = -1; //33 game positions
            ICandidates candidates = getSquareBoard();
            int NON_LEGAL = candidates.TokenIllegalPosition;

            //init mappers
            foreach (var indexModel in candidates
                .EnumerateNodesByIndex(x => x.Content != NON_LEGAL))
            {
                ++indexInGame;
                mapIndexModelToGame.Add(indexModel, indexInGame);
            }

            return mapIndexModelToGame;
        }

        private void buildMoves(
            int[] source, int[] jumped, int[] target, int indexModel, ref uint indexMove,
            Dictionary<int, Solver2Move1> moves,
            Dictionary<int, int> mapIndexModelToGame
            )
        {
            ushort id = 0;
            int indexGame = 0;
            int direction = 0;
            for (int nswe = 0; nswe < 4; nswe++)//nswe 0123
            {
                if (source[nswe] != -1 && jumped[nswe] != -1 && target[nswe] != -1)
                {
                    //add move
                    indexGame = mapIndexModelToGame[indexModel];
                    direction = nswe;
                    id = (ushort)(direction * 100 + indexGame);

                    moves.Add(id, new Solver2Move1()
                    {
                        IndexMove = (ushort)indexMove,
                        IndexGame = (ushort)indexGame,
                        ID = id,
                        Direction = (byte)nswe,
                        Mask = 0,
                        PostMove = 0,
                        PreMove = 0,
                        StorageBitsMaskID = 0,
                        StorageBitsValueID = 0

                    });

                    indexMove++;
                }
            }
        }


#if SOURCE_HAS_TEST_DEF
        [TestMethod]
        public void TestInitMovesIDIndexsAndDirection()
        {
            ICandidates candidates = getSquareBoard();
            SolverBoard solverBoard = new SolverBoard(getLegalPositions());
            MoveController moveController = getMovesController();

            Dictionary<int, MoveSolver2> moves = moveController.Moves;

            Assert.AreEqual(moves.Count, 76, "Error in number of moves");

            int[][] ids = new int[][]{
                new int[] { 1 * 100 + 0, 0},//moveID, moveIndex
                new int[] { 3 * 100 + 0,1},
                new int[] { 0 * 100 + 15,32},
                new int[] { 1 * 100 + 15,33},
                new int[] { 2 * 100 + 15,34},
                new int[] { 3 * 100 + 15,35},
                new int[] { 0 * 100 + 32,74},
                new int[] { 2 * 100 + 32,75},
                new int[] { 3 * 100 + 13,30},
                new int[] { 2 * 100 + 19,45}
            };
            
            for (int i = 0; i < ids.Length; i++)
            {
                
                Assert.IsTrue(moves[ids[i][0]]!=null, "error move exists " + ids[i][0].ToString());
                Assert.AreEqual((ushort)(ids[i][0]), moves[ids[i][0]].ID, "error id value " + ids[i].ToString());
                Assert.AreEqual((ushort)(ids[i][0] % 100), moves[ids[i][0]].IndexGame,  "error indexGame value " + ids[i].ToString());
                Assert.AreEqual((byte)((ids[i][0] - (ids[i][0] % 100)) / 100), moves[ids[i][0]].Direction, "error nswe direction " + ids[i].ToString());
                Assert.AreEqual(ids[i][1], moves[ids[i][0]].IndexMove,  "error indexMove value " + ids[i].ToString());
            }
        }
#endif//SOURCE_HAS_TEST_DEF


#if SOURCE_HAS_TEST_DEF
        [TestMethod]
        public void TestInitMovesMask()
        {
            ICandidates candidates = getSquareBoard();
            SolverBoard solverBoard = new SolverBoard(getLegalPositions());
            MoveController moveController = getMovesController();
            
            Dictionary<int, MoveSolver2> moves = moveController.Moves;

            display(moves);
        
            SolverTestData stData = new SolverTestData();
            
            MoveSolver2 move = new MoveSolver2();


            foreach (var item in stData.MaskData)
            {
                move = moveController.Moves[item.Direction * 100 + item.GameIndex];
                item.SplitMaskPrePost();

                Assert.AreEqual(item.Direction, move.Direction,  "direction error");
                Assert.AreEqual(item.GameIndex, move.IndexGame, "gameIndex error");
                Assert.AreEqual(convertStringBitMasksFromLeft(item.Mask,solverBoard), move.MaskMove, "mask error");
                Assert.AreEqual(convertStringBitMasksFromLeft(item.Pre,solverBoard), move.PreMove, "preMove error");
                Assert.AreEqual(convertStringBitMasksFromLeft(item.Post,solverBoard), move.PostMove, "postMove error");
            }
        }
#endif//SOURCE_HAS_TEST_DEF


        ulong convertStringBitMasksFromLeft(string bits, SolverBoard sb)
        {
            int index = -1;
            ulong mask = 0;

            foreach (var item in bits)//left to right
            {
                if (item.Equals('1'))
                {
                    index++;
                    sb.setBit(index, ref mask);
                }
                if (item.Equals('0'))
                {
                    index++;
                }
            }

            return mask;
        }

        [Conditional("DEBUG")]
        private void display<T>(IEnumerable<T> value)
        {
            foreach (var item in value)
            {
                Debug.WriteLine(item);
            }
        }


#if SOURCE_HAS_TEST_DEF
        
#endif

        /*
                    0	1	2		
		            3	4	5		
            6	7	8	9	10	11	12
            13	14	15	16	17	18	19
            20	21	22	23	24	25	26
		            27	28	29		
		            30	31	32		

            */

#if SOURCE_HAS_TEST_DEF
        [TestMethod]
        public void TestMoveLookupInit()
        {
            ICandidates candidates = getSquareBoard();
            SolverBoard solverBoard = new SolverBoard(getLegalPositions());
            MoveController moveController = getMovesController();

            ushort[][] moveLookup = moveController.MoveLookup;
            Assert.IsTrue(moveLookup != null, "movelookup is null");
            Assert.AreEqual(33, moveLookup.Length, "expected number board positions having moves is in error");
            Assert.AreEqual(4, moveLookup[8].Length, "expected number moves index 8 in error");
            Assert.AreEqual(4, moveLookup[16].Length, "expected number moves index 16 in error");
            Assert.AreEqual(4, moveLookup[23].Length, "expected number moves index 23 in error");
            Assert.AreEqual(1, moveLookup[1].Length, "expected number moves index 1 in error");
            Assert.AreEqual(1, moveLookup[13].Length, "expected number moves index 13 in error");
            Assert.AreEqual(1, moveLookup[19].Length, "expected number moves index 19 in error");
            Assert.AreEqual(2, moveLookup[30].Length, "expected number moves index 30 in error");
            Assert.AreEqual(2, moveLookup[0].Length, "expected number moves index 0 in error");
            Assert.AreEqual(2, moveLookup[29].Length, "expected number moves index 29 in error");
        }
#endif

        /*
         * 0,0,1,1,1,0,0,//6 - (0-2)
         * 0,0,1,1,1,0,0,//13 - (3-5)
         * 1,1,1,1,1,1,1,//20 - (6-12)
         * 1,1,1,0,1,1,1,//27 - (13-19)
         * 1,1,1,1,1,1,1,//34 - (20-26)
         * 0,0,1,1,1,0,0,//41 - (27-29)
         * 0,0,1,1,1,0,0//48 -  (30-32)
         */
        [TestMethod]
        public void TestNumberOfMovesByGamePos()
        {
            ICandidates candidates = getSquareBoard();
            SolverBoard solverBoard = new SolverBoard(getLegalPositions());
            MoveController moveController = getMovesController();

            Assert.AreEqual(2, moveController.NumberMovesPerGamePos[0], "expected number of moves in error");
            Assert.AreEqual(2, moveController.NumberMovesPerGamePos[6], "expected number of moves in error");
            Assert.AreEqual(2, moveController.NumberMovesPerGamePos[32], "expected number of moves in error");
            Assert.AreEqual(2, moveController.NumberMovesPerGamePos[6], "expected number of moves in error");
            Assert.AreEqual(1, moveController.NumberMovesPerGamePos[1], "expected number of moves in error");
            Assert.AreEqual(1, moveController.NumberMovesPerGamePos[18], "expected number of moves in error");
            Assert.AreEqual(4, moveController.NumberMovesPerGamePos[8], "expected number of moves in error");
            Assert.AreEqual(4, moveController.NumberMovesPerGamePos[16], "expected number of moves in error");
            Assert.AreEqual(4, moveController.NumberMovesPerGamePos[24], "expected number of moves in error");
        }

        
        

    }
}
