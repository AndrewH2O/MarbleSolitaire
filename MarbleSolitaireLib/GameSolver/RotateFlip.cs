#define CHATTY
#undef CHATTY
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{
    public class RotateFlip
    {
        int SIDE = 7;
        int _length;
        int _bitsLength;
        int _rotationsFlips;

        public int RotationsFlips
        {
            get { return _rotationsFlips; }
            
        }
        /*
        0	1	2	3	4	5	6
        7	8	9	10	11	12	13
        14	15	16	17	18	19	20
        21	22	23	24	25	26	27
        28	29	30	31	32	33	34
        35	36	37	38	39	40	41
        42	43	44	45	46	47	48
         */

        string _data =
            " , ,o,.,., , ," +
            " , ,o,.,., , ," +
            ".,.,o,o,o,o,o," +
            ".,.,.,.,o,o,o," +
            ".,.,.,.,o,o,o," +
            " , ,.,.,o, , ," +
            " , ,.,.,o, , ";

        int[] _nonLegal = new int[] { 0, 1, 5, 6, 7, 8, 12, 13, 35, 36, 40, 41, 42, 43, 47, 48 };
        
        //use 7 by 7 square format
        //L long ie Model count, R Rotation F Flip 
        int[] _indexesL0;
        int[] _indexesLR90;
        int[] _indexesLR180;
        int[] _indexesLR270;
        int[] _indexesLR360;

        int[] _indexesLFlipY;
        int[] _indexesLFlipX;

        int[] _indexesLFR90;
        int[] _indexesLFR180;
        int[] _indexesLFR270;
        int[] _indexesLFR360;
        
        //33 positions
        int[,] _indexesPacked;

        public int[,] IndexesPacked
        {
            get { return _indexesPacked; }
        }

        Mapper _mapper;

        public RotateFlip(Mapper mapper)
        {
            if (mapper != null)
            {
                _mapper = mapper;
            }
            _length = SIDE * SIDE;
            doOriginal();
            doRotateBy90();
            doRotateBy180();
            doRotateBy270();
            doRotateBy360();
            doFlipX();
            doFlipY();
            //flip X rotate = flip Y rotate 
            doFlipRotateBy90();
            doFlipRotateBy180();
            doFlipRotateBy270();
            doFlipRotateBy360();
            //Debug.WriteLine(displayIndexes());

            _bitsLength = 33;
            _rotationsFlips = 8;
            _indexesPacked = new int[_rotationsFlips, _bitsLength];
            
            buildPackedIndexes();
#if CHATTY
            Debug.WriteLine("packed indexes");
            Debug.WriteLine("LR360, LFR180, LFR90, LR270, LR90, LFR270, LFR360, LR180");
            Debug.WriteLine(displayIndexesPacked());
#endif
            _rotatedFlippedBits = new ulong[_rotationsFlips];
        }

        /// <summary>
        /// convert indexes from model 7 by 7 format to 33 game format
        /// </summary>
        private void buildPackedIndexes()
        {
            if (_mapper == null) return;
            int NON_LEGAL=-1;
            int index = -1;
            for (int i = 0; i < _length; i++)
            {
                if(_mapper.GetModelToGameByIndex(i)==NON_LEGAL)continue;
                index++;
                _indexesPacked[0, index] = _mapper.MapIndexModelToGame[_indexesLR360[i]];
                _indexesPacked[1, index] = _mapper.MapIndexModelToGame[_indexesLFR180[i]];
                _indexesPacked[2, index] = _mapper.MapIndexModelToGame[_indexesLFR90[i]];
                _indexesPacked[3, index] = _mapper.MapIndexModelToGame[_indexesLR270[i]];
                _indexesPacked[4, index] = _mapper.MapIndexModelToGame[_indexesLR90[i]];
                _indexesPacked[5, index] = _mapper.MapIndexModelToGame[_indexesLFR270[i]];
                _indexesPacked[6, index] = _mapper.MapIndexModelToGame[_indexesLFR360[i]];
                _indexesPacked[7, index] = _mapper.MapIndexModelToGame[_indexesLR180[i]];
                
                //_indexesPacked[0, index] = _mapper.MapIndexModelToGame[_indexesLR90[i]];
                //_indexesPacked[1, index] = _mapper.MapIndexModelToGame[_indexesLR180[i]];
                //_indexesPacked[2, index] = _mapper.MapIndexModelToGame[_indexesLR270[i]];
                //_indexesPacked[3, index] = _mapper.MapIndexModelToGame[_indexesLR360[i]];
                //_indexesPacked[4, index] = _mapper.MapIndexModelToGame[_indexesLFR90[i]];
                //_indexesPacked[5, index] = _mapper.MapIndexModelToGame[_indexesLFR180[i]];
                //_indexesPacked[6, index] = _mapper.MapIndexModelToGame[_indexesLFR270[i]];
                //_indexesPacked[7, index] = _mapper.MapIndexModelToGame[_indexesLFR360[i]];
                
            }
        }

        private string displayIndexesPacked()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _rotationsFlips; i++)
            {
                for (int j = 0; j < _bitsLength; j++)
                {
                    sb.Append(_indexesPacked[i, j]);
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        private string displayIndexes()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(displayIndex(_indexesLR90));
            sb.Append("\n");
            sb.Append(displayIndex(_indexesLR180));
            sb.Append("\n");
            sb.Append(displayIndex(_indexesLR270));
            sb.Append("\n");
            sb.Append(displayIndex(_indexesLR360));
            sb.Append("\n");
            sb.Append(displayIndex(_indexesLFR90));
            sb.Append("\n");
            sb.Append(displayIndex(_indexesLFR180));
            sb.Append("\n");
            sb.Append(displayIndex(_indexesLFR270));
            sb.Append("\n");
            sb.Append(displayIndex(_indexesLFR360));
            sb.Append("\n");
            return sb.ToString();

            
        }

        private string displayIndex(int[] array)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                sb.Append(array[i]);
                sb.Append(",");
            }
            return sb.ToString();
        }

        public void TestData()
        {
            
        }

        private void doOriginal()
        {
            _indexesL0 = new int[_length];
            for (int i = 0; i < _length; i++)
            {
                _indexesL0[i] = i;
            }
        }

        public enum RFState { ORIG, R90, R180, R270, R360, FLX, FLY, FR90, FR180, FR270, FR360 };

        public string DisplayTestData()
        {
            return convertToDisplay(_data);
        }

        public string DisplayTestData(string data, RFState rfState)
        {
            string[] s = data.Split(',');
            switch (rfState)
            {
                case RFState.ORIG:
                    return convertToDisplay(data);
                case RFState.R90:
                    return buildAsText(_indexesLR90, s);
                case RFState.R180:
                    return buildAsText(_indexesLR180, s);
                case RFState.R270:
                    return buildAsText(_indexesLR270, s);
                case RFState.R360:
                    return buildAsText(_indexesLR360, s);
                case RFState.FLX:
                    return buildAsText(_indexesLFlipX, s);
                case RFState.FLY:
                    return buildAsText(_indexesLFlipY, s);
                case RFState.FR90:
                    return buildAsText(_indexesLFR90, s);
                case RFState.FR180:
                    return buildAsText(_indexesLFR180, s);
                case RFState.FR270:
                    return buildAsText(_indexesLFR270, s);
                case RFState.FR360:
                    return buildAsText(_indexesLFR360, s);
                default:
                    break;
            }

            return string.Empty;

        }

        ulong[] _rotatedFlippedBits;

        public ulong[] RotatedFlippedBits
        {
            get { return _rotatedFlippedBits; }
        } 

        /// <summary>
        /// Check bitBoard against 8 possible rotated flipped states
        /// </summary>
        /// <param name="board"></param>
        /// <param name="toMatch"></param>
        /// <returns></returns>
        public bool CheckBits(ulong board, ulong toMatch)
        {
            for (int i = 0; i < _rotationsFlips; i++)
            {
                _rotatedFlippedBits[i] = 0;
                for (int j = 0; j < _bitsLength; j++)
                {
                    //index = _indexesPacked[i,j];
                    if(testBit(j,board))
                    {
                        setBit(_indexesPacked[i, j], ref _rotatedFlippedBits[i]);
                    }
                }
                if (_rotatedFlippedBits[i] == toMatch) return true;
            }
            return false;
        }

        

        public void GetRotationsFlipsForBoard(ulong board, ulong[]boards)
        {
            for (int i = 0; i < _rotationsFlips; i++)
            {
                boards[i] = 0;
                //_rotatedFlippedBits[i] = 0;
                for (int j = 0; j < _bitsLength; j++)
                {
                    //index = _indexesPacked[i,j];
                    //if (testBit(j, board))
                    if((board & (1ul << j)) != 0)
                    {
                        boards[i] |= (1ul << _indexesPacked[i, j]);
                        //setBit(_indexesPacked[i, j], ref _rotatedFlippedBits[i]);
                    }
                }
                
            }
            //return _rotatedFlippedBits;
        }


        public void GetRotationsFlipsForBoard(ulong board, List<ulong> boards)
        {
            for (int i = 0; i < _rotationsFlips; i++)
            {
                boards[i] = 0;
                //_rotatedFlippedBits[i] = 0;
                for (int j = 0; j < _bitsLength; j++)
                {
                    //index = _indexesPacked[i,j];
                    //if (testBit(j, board))
                    if ((board & (1ul << j)) != 0)
                    {
                        boards[i] |= (1ul << _indexesPacked[i, j]);
                        //setBit(_indexesPacked[i, j], ref _rotatedFlippedBits[i]);
                    }
                }

            }
            //return _rotatedFlippedBits;
        }

        private void doFlipX()
        {
            int x, y, index;
            //int side = 4;
            _indexesLFlipX = new int[_length];
            for (int i = 0; i < _length; i++)
            {
                x = (SIDE - 1) - (i % SIDE);
                y = i / SIDE * SIDE;
                index = y + x;
                _indexesLFlipX[i] = index;
            }
        }

        private void doFlipY()
        {
            int x, y, index;
            //int side = 4;
            _indexesLFlipY = new int[_length];
            for (int i = 0; i < _length; i++)
            {
                x = i % SIDE;
                y = (_length - 1 - i) / SIDE * SIDE;
                index = y + x;
                _indexesLFlipY[i] = index;
            }
        }


        private void doRotateBy360()
        {
            if (_indexesL0 == null || _indexesL0.Length == 0)
            {
                doOriginal();
            }
            
            _indexesLR360 = new int[_length];
            for (int i = 0; i < _length; i++)
            {
                _indexesLR360[i] = _indexesL0[i];
            }
        }

        private void doRotateBy270()
        {
            if (_indexesLR90 == null || _indexesLR90.Length == 0)
            {
                doRotateBy90();
            }

            _indexesLR270 = new int[_length];
            for (int i = 0; i < _length; i++)
            {
                _indexesLR270[i] = _indexesLR90[_length - 1 - i];
            }
        }

        private void doRotateBy180()
        {
            if (_indexesL0 == null || _indexesL0.Length == 0)
            {
                doOriginal();
            }

            if (_indexesLR180 == null || _indexesLR180.Length == 0)
            {
                _indexesLR180 = new int[_length];
                for (int i = 0; i < _length; i++)
                {
                    _indexesLR180[i] = _indexesL0[_length - 1 - i];
                }
            }
            
        }

        

        private void doRotateBy90()
        {
            if (_indexesL0 == null || _indexesL0.Length == 0)
            {
                doOriginal();
            }

            if (_indexesLR90 == null || _indexesLR90.Length == 0)
            {
                _indexesLR90 = new int[_length];
                int x, y, index;
                //int side = 4;
                int offset = SIDE - 1;
                for (int i = 0; i < _length; i++)
                {
                    //original[i]
                    index = _indexesL0[i];
                    x = index % SIDE;
                    y = index / SIDE;
                    offset = SIDE - 1 - x;
                    _indexesLR90[index] = offset * SIDE + y;
                    
                }
            }
            
        }


        private void doFlipRotateBy360()
        {
            if (_indexesLFlipY == null || _indexesLFlipY.Length == 0)
            {
                doFlipY();
            }

            _indexesLFR360 = new int[_length];
            for (int i = 0; i < _length; i++)
            {
                _indexesLFR360[i] = _indexesLFlipY[i];
            }
        }


        private void doFlipRotateBy270()
        {
            if (_indexesLFR90 == null || _indexesLFR90.Length == 0)
            {
                doFlipRotateBy90();
            }

            _indexesLFR270 = new int[_length];
            for (int i = 0; i < _length; i++)
            {
                _indexesLFR270[i] = _indexesLFR90[_length - 1 - i];
            }
        }

        private void doFlipRotateBy180()
        {
            if (_indexesLFlipY == null || _indexesLFlipY.Length == 0)
            {
                doFlipY();
            }

            if (_indexesLFR180 == null || _indexesLFR180.Length == 0)
            {
                _indexesLFR180 = new int[_length];
                for (int i = 0; i < _length; i++)
                {
                    _indexesLFR180[i] = _indexesLFlipY[_length - 1 - i];
                }
            }

        }

        private void doFlipRotateBy90()
        {
            if (_indexesLR90 == null || _indexesLR90.Length == 0)
            {
                doRotateBy90();
            }

            if (_indexesLFlipY == null || _indexesLFlipY.Length == 0)
            {
                doFlipY();
            }

            if (_indexesLFR90 == null || _indexesLFR90.Length == 0)
            {
                _indexesLFR90 = new int[_length];
                for (int i = 0; i < _length; i++)
                {
                    _indexesLFR90[i] = _indexesLFlipY[_indexesLR90[i]];
                }
            }

        }

        string buildAsText(int[] array, string[] source)
        {
            StringBuilder sb = new StringBuilder();
            int length = array.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                sb.Append(source[array[length - 1 - i]]);
                if (((i) % SIDE) == 0)
                    sb.Append("\n");
            }
            return sb.ToString();
        }

        private string convertToDisplay(string data)
        {
            string[] s = data.Split(',');
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                sb.Append(s[i]);
                if (((i + 1) % SIDE) == 0)
                    sb.Append("\n");
            }
            return sb.ToString();
        }

        private string convertDataBackToCommaDelim(string data)
        {
            string[] s = data.Split('\n');
            string sUnsplit = string.Empty;
            foreach (var item in s)
            {
                sUnsplit += item;
            }

            char[] c = sUnsplit.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < c.Length; i++)
            {
                sb.Append(c[i]);
                sb.Append(",");

            }
            sb.Remove(2 * c.Length - 1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// test bit at given index - used without error checking for speed
        /// </summary>
        /// <param name="index"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        bool testBit(int index, UInt64 board)
        {
            return (((board & (1ul << index)) != 0) ? true : false);
        }

        public void setBit(int index, ref UInt64 bits)
        {
            
            bits |= (1ul << index);
        }

        public void clearBit(int index, ref UInt64 bits)
        {
            
            bits &= ~(1ul << index);
        }
    }
}
