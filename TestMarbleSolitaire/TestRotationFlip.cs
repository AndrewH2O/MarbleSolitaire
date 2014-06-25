using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TestMarbleSolitaire.Helpers;
using System.Collections.Generic;
using MarbleSolitaireModelLib.Model;
using MarbleSolitaireLib.GameSolver;

namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestRotationFlip
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


        #region data
        string data =
            " , ,o,.,., , ," +
            " , ,o,.,., , ," +
            ".,.,o,o,o,o,o," +
            ".,.,.,.,o,o,o," +
            ".,.,.,.,o,o,o," +
            " , ,.,.,o, , ," +
            " , ,.,.,o, , ";

        //int SIDE = 7;
        string expectedOrig = "  o..  \n  o..  \n..ooooo\n....ooo\n....ooo\n  ..o  \n  ..o  \n";


        string expectedR90  = "  ...  \n  ...  \n....ooo\n....o..\nooooo..\n  ooo  \n  ooo  \n";
        string expectedR180 = "  o..  \n  o..  \nooo....\nooo....\nooooo..\n  ..o  \n  ..o  \n";
        string expectedR270 = "  ooo  \n  ooo  \n..ooooo\n..o....\nooo....\n  ...  \n  ...  \n";
        string expectedR360 = "  o..  \n  o..  \n..ooooo\n....ooo\n....ooo\n  ..o  \n  ..o  \n";
        
        string expectedFlipY= "  ..o  \n  ..o  \n....ooo\n....ooo\n..ooooo\n  o..  \n  o..  \n";
        string expectedFlipX= "  ..o  \n  ..o  \nooooo..\nooo....\nooo....\n  o..  \n  o..  \n";

        string expectedFR90 = "  ...  \n  ...  \nooo....\n..o....\n..ooooo\n  ooo  \n  ooo  \n";
        string expectedFR180= "  ..o  \n  ..o  \nooooo..\nooo....\nooo....\n  o..  \n  o..  \n";
        string expectedFR270= "  ooo  \n  ooo  \nooooo..\n....o..\n....ooo\n  ...  \n  ...  \n";
        string expectedFR360= "  ..o  \n  ..o  \n....ooo\n....ooo\n..ooooo\n  o..  \n  o..  \n";

        
        List<int> expectedR90asList;  
        List<int> expectedR180asList;         
        List<int> expectedR270asList;
        List<int> expectedR360asList;
         
        List<int> expectedFlipYasList;
        List<int> expectedFlipXasList;
        
        List<int> expectedFR90asList; 
        List<int> expectedFR180asList;
        List<int> expectedFR270asList;
        List<int> expectedFR360asList;

        
        #endregion

        [TestMethod]
        public void TestCalculationRotations()
        {
            RotateFlip rf = new RotateFlip(null);
            Assert.AreEqual(expectedOrig, rf.DisplayTestData(data,RotateFlip.RFState.ORIG), "Test data not as expected");
            Assert.AreEqual(expectedR90, rf.DisplayTestData(data, RotateFlip.RFState.R90), "Rotate 90 in error");
            Assert.AreEqual(expectedR180, rf.DisplayTestData(data, RotateFlip.RFState.R180), "Rotate 180 in error");
            Assert.AreEqual(expectedR270, rf.DisplayTestData(data, RotateFlip.RFState.R270), "Rotate 270 in error");
            Assert.AreEqual(expectedR360, rf.DisplayTestData(data, RotateFlip.RFState.R360), "Rotate 360 in error");
        }

        [TestMethod]
        public void TestExpectedFlips()
        {
            RotateFlip rf = new RotateFlip(null);
            Assert.AreEqual(expectedFlipY, rf.DisplayTestData(data, RotateFlip.RFState.FLY), "Flip Y in error");
            Assert.AreEqual(expectedFlipX, rf.DisplayTestData(data, RotateFlip.RFState.FLX), "Flip X in error");
        }

        [TestMethod]
        public void TestCalculationFlipRotations()
        {
            RotateFlip rf = new RotateFlip(null);
            Assert.AreEqual(expectedFR90, rf.DisplayTestData(data, RotateFlip.RFState.FR90), "Rotate 90 in error");
            Assert.AreEqual(expectedFR180, rf.DisplayTestData(data, RotateFlip.RFState.FR180), "Rotate 180 in error");
            Assert.AreEqual(expectedFR270, rf.DisplayTestData(data, RotateFlip.RFState.FR270), "Rotate 270 in error");
            Assert.AreEqual(expectedFR360, rf.DisplayTestData(data, RotateFlip.RFState.FR360), "Rotate 360 in error");
        }

        [TestMethod]
        public void TestPackedIndexes()
        {
            Mapper mapper = new Mapper(getSquareBoard());
            RotateFlip rf = new RotateFlip(mapper);
            Assert.IsTrue(rf.IndexesPacked!=null,"Packedindexes should not be non null");
            int numberRotationsFlips = 8;
            int numberGamePosition = 33;
            Assert.IsTrue(rf.IndexesPacked.GetLength(0) == numberRotationsFlips, "rank 0 error");
            Assert.IsTrue(rf.IndexesPacked.GetLength(1) == numberGamePosition, "rank 1 error");
        }

        
        [TestMethod]
        public void TestRotatingBitsAgainstListInput()
        {
            Solver2 solver = new Solver2(getSquareBoard());
            solver.LoadBoard(getStart(), LoadState.Start);
            ulong board = solver.CurrentBoard;
            RotateFlip rf = new RotateFlip(new Mapper(getSquareBoard()));
            rf.CheckBits(board, 0);
            ulong[] rotatedFlippedBits = rf.RotatedFlippedBits;

            buildExpectedBitsAsListFromStrings();
            testRFBits(rotatedFlippedBits[0], expectedR360asList, solver);
            //testRFBits(rotatedFlippedBits[1], expectedR180asList, solver);
            //testRFBits(rotatedFlippedBits[2], expectedR270asList, solver);
            //testRFBits(rotatedFlippedBits[3], expectedR360asList, solver);

            //testRFBits(rotatedFlippedBits[4], expectedFR90asList, solver);
            //testRFBits(rotatedFlippedBits[5], expectedFR180asList, solver);
            //testRFBits(rotatedFlippedBits[6], expectedFR270asList, solver);
            //testRFBits(rotatedFlippedBits[7], expectedFR360asList, solver);

            Assert.IsTrue(testRFBits(rotatedFlippedBits[0], expectedR360asList, solver), "R360 error");
            Assert.IsTrue(testRFBits(rotatedFlippedBits[1], expectedFR180asList, solver), "FR180 error");
            Assert.IsTrue(testRFBits(rotatedFlippedBits[2], expectedFR90asList, solver), "FR90 error");
            Assert.IsTrue(testRFBits(rotatedFlippedBits[3], expectedR90asList, solver), "R270 error");

            Assert.IsTrue(testRFBits(rotatedFlippedBits[4], expectedR270asList, solver), "R90 error");
            Assert.IsTrue(testRFBits(rotatedFlippedBits[5], expectedFR270asList, solver), "FR270 error");
            Assert.IsTrue(testRFBits(rotatedFlippedBits[6], expectedFR360asList, solver), "FR360 error");
            Assert.IsTrue(testRFBits(rotatedFlippedBits[7], expectedR180asList, solver), "R180 error");

        }


        private bool testRFBits(ulong bits, List<int> board, Solver2 solver)
        {
            solver.LoadBoard(board, LoadState.Start);
            ulong currentBoard = solver.CurrentBoard;
            return currentBoard == bits;

        }


        [TestMethod]
        public void TestRotatingBitsBitValues()
        {
            Solver2 solver = new Solver2(getSquareBoard());
            solver.LoadBoard(getStart(), LoadState.Start);
            ulong board = solver.CurrentBoard;
            RotateFlip rf = new RotateFlip(new Mapper(getSquareBoard()));
            rf.CheckBits(board, 0);
            ulong[] rotatedFlippedBits = rf.RotatedFlippedBits;

            Assert.AreEqual(7380799uL, rotatedFlippedBits[4], "bit values R90 error");
            Assert.AreEqual(4864401865uL, rotatedFlippedBits[7], "bit values R180 error");
            Assert.AreEqual(8488360960uL, rotatedFlippedBits[3], "bit values R270 error");
            Assert.AreEqual(4950204169uL, rotatedFlippedBits[0], "bit values R360 error");

            Assert.AreEqual(8585773504uL, rotatedFlippedBits[2], "bit values FR90 error");
            Assert.AreEqual(1215358948uL, rotatedFlippedBits[1], "bit values FR180 error");
            Assert.AreEqual(117573631uL, rotatedFlippedBits[5], "bit values FR270 error");
            Assert.AreEqual(1338907684uL, rotatedFlippedBits[6], "bit values FR360 error");

        }

        [TestMethod]
        public void TestMatchedBits()
        {
            List<Tuple<ulong, ulong,bool>> data = new List<Tuple<ulong, ulong, bool>>()
            {
                //board, match with, expected is match
                new Tuple<ulong, ulong,bool> (4950204169,117573631,true),
                new Tuple<ulong, ulong,bool> (4864401865,4864401865,true),
                new Tuple<ulong, ulong,bool> (67174930,4295090176,true),
                new Tuple<ulong, ulong,bool> (67174930,68223506,false),
                new Tuple<ulong, ulong,bool> (16843008,29591296,false),
                new Tuple<ulong, ulong,bool> (4324558596,1103333121,true),
                new Tuple<ulong, ulong,bool> (29595456,1103333121,true),
                new Tuple<ulong, ulong,bool> (0,0,true),
                new Tuple<ulong, ulong,bool> (8488413183,8586731327,false),
                new Tuple<ulong, ulong,bool> (1312585983,4936464639,true),
                new Tuple<ulong, ulong,bool> (8560343241,1312585983,true),
            };

            RotateFlip rf = new RotateFlip(new Mapper(getSquareBoard()));
            
            int index=-1;
            foreach (var item in data)
            {
                index++;
                Assert.AreEqual(item.Item3, rf.CheckBits(item.Item1, item.Item2), "match item:" + index.ToString() + " in error");
            }
        }

        [TestMethod]
        public void TestGetRotationsFlipsForBoard()
        {
            List<Tuple<ulong, ulong, bool>> data = new List<Tuple<ulong, ulong, bool>>()
            {
                //board, match with, expected is match
                new Tuple<ulong, ulong,bool> (4950204169,117573631,true),
                new Tuple<ulong, ulong,bool> (4864401865,4864401865,true),
                new Tuple<ulong, ulong,bool> (67174930,4295090176,true),
                new Tuple<ulong, ulong,bool> (67174930,68223506,false),
                new Tuple<ulong, ulong,bool> (16843008,29591296,false),
                new Tuple<ulong, ulong,bool> (4324558596,1103333121,true),
                new Tuple<ulong, ulong,bool> (29595456,1103333121,true),
                new Tuple<ulong, ulong,bool> (0,0,true),
                new Tuple<ulong, ulong,bool> (8488413183,8586731327,false),
                new Tuple<ulong, ulong,bool> (1312585983,4936464639,true),
                new Tuple<ulong, ulong,bool> (8560343241,1312585983,true),
            };

            RotateFlip rf = new RotateFlip(new Mapper(getSquareBoard()));
            int index = -1;
            ulong[] rotatedFlipped=new ulong[8];
            bool isActual;
            foreach (var item in data)
            {
                index++;
                rf.GetRotationsFlipsForBoard(item.Item1,rotatedFlipped);
                isActual = false;
                foreach (var itemRF in rotatedFlipped)
                {
                    if (itemRF == item.Item2)
                    {
                        isActual = true;
                        continue;
                    }
                    
                }
                Assert.AreEqual(item.Item3, isActual, "match item:" + index.ToString() + " in error");
                
            }
        }


        List<int> convertStringBoardToList(string board)
        {
            char[] c = board.ToCharArray();
            int length = 49;
            List<int> result = new List<int>(length);
            int index=-1;
            for (int i = 0; i < c.Length; i++)
            {
                if(c[i]=='.'||c[i]==' ')
                {
                    index++;
                    result.Add(0);
                }
                if (c[i] == 'o')
                {
                    index++;
                    result.Add(1);
                }
            }
            return result;
        }

        void buildExpectedBitsAsListFromStrings()
        {
            expectedR90asList = convertStringBoardToList(expectedR90);
            expectedR180asList = convertStringBoardToList(expectedR180); 
            expectedR270asList = convertStringBoardToList(expectedR270); 
            expectedR360asList = convertStringBoardToList(expectedR360); 

            expectedFlipYasList = convertStringBoardToList(expectedFlipY); 
            expectedFlipXasList = convertStringBoardToList(expectedFlipX); 

            expectedFR90asList = convertStringBoardToList(expectedFR90);
            expectedFR180asList = convertStringBoardToList(expectedFR180);
            expectedFR270asList = convertStringBoardToList(expectedFR270);
            expectedFR360asList = convertStringBoardToList(expectedFR360);
        }

        public Tuple<ulong, ulong> List { get; set; }
    }
}
