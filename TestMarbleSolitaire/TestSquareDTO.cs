using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using MarbleSolitaireModelLib.Model;
using MarbleSolCommonLib.Common;


namespace TestMarbleSolitaire
{
    [TestClass]
    public class TestSquareDTO
    {
        [TestMethod]
        public void TestUpdateLinks()
        {
            //arrange
            int side = 7;
            SquareDTO dto = new SquareDTO(  
                new BitBoard(new List<int>() 
                    { 
                        0,0,1,1,1,0,0, //6
                        0,0,1,1,1,0,0, //13
                        1,1,1,1,1,1,1, //20
                        1,1,1,1,1,1,1, //27
                        1,1,1,1,1,1,1, //34
                        0,0,1,1,1,0,0, //41
                        0,0,1,1,1,0,0  //48
                    }),
                side);

            Node4 n = new Node4();
            int row = 2;
            int col = 3;
            int index = row * side + col;
            
            //act
            dto.UpdateLxInNode(n, row, col);

            //assert
            int[] source = new int[] { 17, 17, 17, 17 };
            int[] jumps = new int[] { 17-7, 17+7, 17-1,17+1 };//NSWE
            int[] targets = new int[] {17-14,17+14,17-2,17+2 };//NSWE

            for (int i = 0; i < source.Length; i++)
            {
                Assert.AreEqual(source[i], n.Source[i],"node source expectations are in error");
                Assert.AreEqual(jumps[i], n.Jumped[i], "node jumped expectations are in error");
                Assert.AreEqual(targets[i], n.Targets[i], "node targets expectations are in error");
            }
        }

        [TestMethod]
        public void TestUpdateLinksWhereSourceIllegal()
        {
            //arrange
            int side = 7;
            SquareDTO dto = new SquareDTO(
                new BitBoard(new List<int>() 
                    { 
                        0,0,1,1,1,0,0, //6
                        0,0,1,1,1,0,0, //13
                        1,1,1,1,1,1,1, //20
                        1,1,1,1,1,1,1, //27
                        1,1,1,1,1,1,1, //34
                        0,0,1,1,1,0,0, //41
                        0,0,1,1,1,0,0  //48
                    }),
                side);

            Node4 n = new Node4();
            int row = 0;
            int col = 1;
            int index = row * side + col;

            //act
            dto.UpdateLxInNode(n, row, col);

            //assert
            
            int[] source = new int[] { Node4.EMPTY_LINK, Node4.EMPTY_LINK, Node4.EMPTY_LINK, Node4.EMPTY_LINK };
            int[] jumps = new int[] { Node4.EMPTY_LINK, Node4.EMPTY_LINK, Node4.EMPTY_LINK, 1+1 };//NSWE
            int[] targets = new int[] { Node4.EMPTY_LINK, 1+14, Node4.EMPTY_LINK, 1+2 };//NSWE

            for (int i = 0; i < source.Length; i++)
            {
                Assert.AreEqual(source[i], n.Source[i],"node source expectations are in error");
                Assert.AreEqual(jumps[i], n.Jumped[i], "node jumped expectations are in error");
                Assert.AreEqual(targets[i], n.Targets[i],"node targets expectations are in error");
            }
        }

        [TestMethod]
        public void TestUpdateLinksWhereSomeTargetIllegal()
        {
            //arrange
            int side = 7;
            SquareDTO dto = new SquareDTO(
                new BitBoard(new List<int>() 
                    { 
                        0,0,1,1,1,0,0, //6
                        0,0,1,1,1,0,0, //13
                        1,1,1,1,1,1,1, //20
                        1,1,1,0,1,1,1, //27
                        1,1,1,1,1,1,1, //34
                        0,0,1,1,1,0,0, //41
                        0,0,1,1,1,0,0  //48
                    }),
                side);

            Node4 n = new Node4();
            int row = 2;
            int col = 1;
            int index = row * side + col;

            //act
            dto.UpdateLxInNode(n, row, col);

            //assert
            int[] source = new int[] { 15,15, 15, 15 };
            int[] jumps = new int[] { Node4.EMPTY_LINK, 15+7, 15-1, 15+1 };//NSWE
            int[] targets = new int[] { Node4.EMPTY_LINK, 15+14, Node4.EMPTY_LINK, 15+2 };//NSWE

            for (int i = 0; i < source.Length; i++)
            {
                Assert.AreEqual(source[i], n.Source[i], "node source expectations are in error");
                Assert.AreEqual(jumps[i], n.Jumped[i],"node jumped expectations are in error");
                Assert.AreEqual(targets[i], n.Targets[i],"node targets expectations are in error");
            }
        }
    }
}
