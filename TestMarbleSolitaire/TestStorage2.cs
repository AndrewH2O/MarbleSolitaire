using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using TestMarbleSolitaire.Helpers;
using MarbleSolitaireModelLib.Model;
using MarbleSolitaireLib.GameSolver;

namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestStorageControllerBits
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

        RotateFlip getRotateFlip()
        {
            return new RotateFlip(new Mapper(getSquareBoard()));
        }

        #endregion//setupHelpers
        
        [TestMethod]
        public void TestInitStorage()
        {
            StorageControllerBits sc = new StorageControllerBits(getRotateFlip());
            Assert.IsTrue(sc != null, "unable to initialise");
        }

        [TestMethod]
        public void TestAddingItemsToStore()
        {
            StorageControllerBits sc = new StorageControllerBits(getRotateFlip());
            Assert.AreEqual(false, sc.CheckIsKnown(1312585983,20), "expected is known error");
            Assert.AreEqual(true, sc.CheckIsKnown(1312585983,20), "expected is known error");

            Assert.AreEqual(false, sc.CheckIsKnown(8589934590,32), "expected is known error");
            Assert.AreEqual(false, sc.CheckIsKnown(0,0), "expected is known error");
            Assert.AreEqual(true, sc.CheckIsKnown(0,0), "expected is known error");
            Assert.AreEqual(false, sc.CheckIsKnown(1073741824,1), "expected is known error");
        }

        //[TestMethod]
        //public void TestIllustratesThatOnlyOneInListBoardsIsSaved()
        //{
        //    StorageController sc = new StorageController();
        //    //false as neither have been
        //    Assert.AreEqual(false, sc.CheckIsKnown(new ulong[] { 8560343241, 1312585983 }), "expected is known error");

        //    //false as although this has been seen it was the last in the above sequence that was
        //    //saved so do not check rotation/flip in isolation as it chnages behaviour of storage
        //    Assert.AreEqual(false, sc.CheckIsKnown(new ulong[] { 8560343241 }), "expected is known error");
        //    //seen and saved as it was last in list items first seen above and so should be true
        //    Assert.AreEqual(true, sc.CheckIsKnown(new ulong[] { 1312585983 }), "expected is known error");

        //    ///TODO look at trade off more memory less cpu in crunching through rotations/flips
        //    ///could save each combination

        //}
    }
}
