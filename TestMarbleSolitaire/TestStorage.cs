using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarbleSolitaireLib.GameSolver;


namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestStorageController
    {
        [TestMethod]
        public void TestInitStorage()
        {
            StorageController sc = new StorageController();
            Assert.IsTrue(sc != null, "unable to initialise");
        }

        [TestMethod]
        public void TestAddingItemsToStore()
        {
            StorageController sc = new StorageController();
            Assert.AreEqual(false, sc.CheckIsKnown(new ulong[] { 1312585983 }), "expected is known error");
            Assert.AreEqual(true, sc.CheckIsKnown(new ulong[] { 1312585983 }), "expected is known error");
            
            Assert.AreEqual(false, sc.CheckIsKnown(new ulong[] { 8589934590 }), "expected is known error");
            Assert.AreEqual(false, sc.CheckIsKnown(new ulong[] { 0 }), "expected is known error");
            Assert.AreEqual(true, sc.CheckIsKnown(new ulong[] { 0 }), "expected is known error");
            Assert.AreEqual(false, sc.CheckIsKnown(new ulong[] { 1073741824 }), "expected is known error");
        }

        [TestMethod]
        public void TestIllustratesThatOnlyOneInListBoardsIsSaved()
        {
            StorageController sc = new StorageController();
            //false as neither have been
            Assert.AreEqual(false, sc.CheckIsKnown(new ulong[] { 8560343241, 1312585983 }), "expected is known error");
            
            //false as although this has been seen it was the last in the above sequence that was
            //saved so do not check rotation/flip in isolation as it chnages behaviour of storage
            Assert.AreEqual(false, sc.CheckIsKnown(new ulong[] { 8560343241 }), "expected is known error");
            //seen and saved as it was last in list items first seen above and so should be true
            Assert.AreEqual(true, sc.CheckIsKnown(new ulong[] { 1312585983 }), "expected is known error");
        
            ///TODO look at trade off more memory less cpu in crunching through rotations/flips
            ///could save each combination
        
        }
    }
}
