using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarbleSolitaireLib.GameSolver;


namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestPiecesController
    {
        [TestMethod]
        public void TestInit()
        {
            ushort numberOfPieces = 32;
            PiecesController pc = new PiecesController(numberOfPieces);
            Assert.IsTrue(pc.PiecesCount == numberOfPieces, "PiecesCount in error");
            Assert.IsTrue(pc.Pieces.Length == numberOfPieces, "Pieces length in error");
            Assert.IsTrue(pc.PiecesStorageBit.Length == numberOfPieces, "PSBF in error");

        }

        [TestMethod]
        public void TestValues()
        {
            ushort numberOfpieces = 32;
            PiecesController pc = new PiecesController(numberOfpieces);

            Assert.AreEqual(1, pc.Pieces[0], "Piece at index 0 is count of 1");
            Assert.AreEqual(32, pc.Pieces[numberOfpieces-1], "Piece at index 31 is count of 32");
            
            Assert.AreEqual((ulong)92358976733184, pc.PiecesStorageBit[21 - 1], "Piece count 21 storageBits error");
            Assert.AreEqual((ulong)61572651155456, pc.PiecesStorageBit[14 - 1], "Piece count 14 storageBits error");
            Assert.AreEqual((ulong)4398046511104, pc.PiecesStorageBit[1 - 1], "Piece count 1 storageBits error");
            Assert.AreEqual((ulong)118747255799808, pc.PiecesStorageBit[27 - 1], "Piece count 27 storageBits error");
            Assert.AreEqual((ulong)140737488355328, pc.PiecesStorageBit[32 - 1], "Piece count 32 storageBits error");
        }
    }

}
