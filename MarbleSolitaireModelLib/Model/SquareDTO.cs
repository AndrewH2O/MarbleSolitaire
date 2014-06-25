
using MarbleSolCommonLib.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireModelLib.Model
{
    /// <summary>
    /// Used to calculate index references into gameboard for moves and interaction
    /// source target and jumped and caches result in a node
    /// </summary>
    public class SquareDTO : DTO
    {
        Node4 _node;
        int _side;
        BitBoard _bitboard;
        NSWE _nswe = new NSWE(0,1,2,3);
        /// <summary>
        /// Used to map logical moves based on source row , col the target and
        /// the index where the piece is that is jumped this is referrred to as
        /// jumps. 
        /// </summary>
        /// <param name="bitBoard"></param>
        /// <param name="side"></param>
        public SquareDTO(BitBoard bitBoard, int side)
        {
            _bitboard = bitBoard;
            _side = side;
        }


        /// <summary>
        /// Start of the update links steps which set the node and cache it. The node
        /// will store for the source index, jumped and target indexes regardless if 
        /// source is legal board position
        /// </summary>
        /// <param name="node"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void UpdateLxInNode(Node4 node, int row, int col)
        {
            this._node = node;
            updateLx(row, col, DataLxType.Source);
            updateLx(row, col, DataLxType.Jumped);
            updateLx(row, col, DataLxType.Target);
        }


        /// <summary>
        /// Update links for Source target and Jumped. The vector calculates
        /// the relevant offset values for row and col and this encapsulates the
        /// relationships or links on the board
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="dataLxType"></param>
        void updateLx(int row, int col, DataLxType dataLxType)
        {
            int vector = -1;
            switch (dataLxType)
            {
                case DataLxType.Target:
                    vector = 2;
                    break;
                case DataLxType.Jumped:
                    vector = 1;
                    break;
                case DataLxType.Source:
                    vector = 0;
                    break;
                default:
                    break;
            }

            //NSWE is 0123 indexing into start jumped target arrays of node respectively
            setLx(row - vector, col, _nswe.N, dataLxType);
            setLx(row + vector, col, _nswe.S, dataLxType);
            setLx(row, col - vector, _nswe.W, dataLxType);
            setLx(row, col + vector, _nswe.E, dataLxType);
        }


        /// <summary>
        /// Set links filling each node array of source, target and jumps for each
        /// possible direction NSWE where the IndexIntoCache hold the relevant 
        /// direction index so index 0 is N, 1 is S, 2 is W and 3 is E
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="col">col</param>
        /// <param name="indexIntoCache">index into array NSWE cached lookup refs on gameboard</param>
        /// <param name="dataLxType">indicates source target or jumps</param>
        private void setLx(int row, int col, int indexIntoCache, DataLxType dataLxType)
        {
            if (!isInBounds(row, col)) return;
            int index = row * _side + col;
            int value = ((isValidGameIndex(index)) ? index : Node4.EMPTY_LINK);
            
            switch (dataLxType)
            {
                case DataLxType.Target:
                    _node.Targets[indexIntoCache] = value;
                    break;
                case DataLxType.Jumped:
                    _node.Jumped[indexIntoCache] = value;
                    break;
                case DataLxType.Source:
                    _node.Source[indexIntoCache] = value;
                    break;
                default:
                    break;
            }
        }

        

        /// <summary>
        /// Checks that we have a valid index within the bounds of the array in which the
        /// game board is defined. e.g row ref are not -ve etc
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool isInBounds(int row, int col)
        {
            if ((row >= 0 && row < _side) && (col >= 0 && col < _side))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks that we have a valid index on the game board.
        /// Before using this check we are within bounds of containng array first
        /// e.g. row ref are not -ve etc
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool isValidGameIndex(int index)
        {
            return !(_bitboard.IsIllegal(index));
        }

        


        enum DataLxType { Target, Jumped, Source }
        
        /// <summary>
        /// indexes into node source,jumped, target array caches
        /// so taking array NSWE.N as the first it indexes into the
        /// first value within the source, jumped and target arrays respectively
        /// of a node. Then S having a value of 1 indexes into the second item of
        /// each array. This is used to maps four way moves.
        /// </summary>
        struct NSWE
        {
            public int N;
            public int S;
            public int W;
            public int E;
            public NSWE(int n, int s, int w, int e)
            {
                N = n; S = s; W = w; E = e;
            }
        }



        
    }
}
