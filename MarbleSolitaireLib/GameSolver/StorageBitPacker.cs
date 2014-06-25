

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{
    
    
    
    /// <summary>
    /// Represent game state as 64bit ulong - used to pack bits for storage
    /// Packs 3 fields board, moveID and Pieces
    /// Bits Packed from the Right 
    /// 0-32 board 1 bit = 1 position; 
    ///     Bits map to board positions so  converting to a value is meaningless
    /// 33-41 moveID 0 to 332 where d * 100 + index 
    ///     where d is nswe direction values 0-3 padded by 100 and 
    ///     index is a position on the gameboard 0-32. 
    ///     so 332 requires 9 bits of storage
    ///     Bits can be converted to values 
    /// 42-47 count of pieces values 0-32 which requires 6 bits of storage
    ///     Bits can be converted to values
    /// Can process strings; board as string read from left to right 
    /// for readability but packed right to left, 
    /// for the other fields the strings should be processed right to left and 
    /// packed the same way.
    /// </summary>
    public class StorageBitPacker
    {
        public enum StorageField { All, Board, MoveID, PiecesCount }
        
        const int MAX_INDEX = 63;
        const int NUMBER_GAME_POSITIONS = 33;
        int NUMBER_BITS_GAME_POS = 33;
        int NUMBER_BITS_MOVEID = 9;
        int NUMBER_BITS_PIECESCOUNT = 6;
        int PACKING_PRE_PIECESCOUNT = 33 + 9;//gamePos+moveID
        int PACKING_PRE_MOVEID = 33;
        int PACKING_PRE_BOARD = 0;
        int END_SIGNIFICANT_BITS_INDEX = 33 + 9 + 6;
        

        //game storage
        ulong _boardMask;//bits 0-32: {B} board pos = 0-32 where each position equals a bit
        ulong _moveIDMask;//bits 33-41: 76 moves as (d)irection * 100 + {B}  where d is 0-3 so max 3*100+32 takes 9 bits ulong value 4389456576512 
        ulong _piecesCountMask;//bits 42-47: max value 32 which takes 5 bits

        ulong _board;
        ulong _moveID;
        ulong _piecesCount;

        ulong _storageBits;

        /// <summary>
        /// Board field packed in storageBits format
        /// eg Pieces (packed with 0s) + MoveID (packed with 0s)  + Board (1s & 0s)
        /// </summary>
        public ulong Board
        {
            get 
            {
                return _board; 
            }
            set
            {
                _board = value;
                _storageBits = Update(_storageBits, _boardMask, _board);
            }
        }
        
        /// <summary>
        /// MoveID field packed in storageBits format
        /// eg Pieces (packed with 0s) + MoveID (1s & 0s)  + Board (packed with 0s)
        /// </summary>
        public ulong MoveID
        {
            get 
            {
                return _moveID; 
            }
            set
            {
                _moveID = value;
                _storageBits = Update(_storageBits, _moveIDMask, _moveID);
            }
        }

        /// <summary>
        /// Pieces count field packed in storageBits format
        /// eg Pieces (1s & 0s) + MoveID (packed with 0s)  + Board (packed with 0s)
        /// </summary>
        public ulong PiecesCount
        {
            get 
            {
                return _piecesCount; 
            }
            set
            {
                _piecesCount = value;
                _storageBits = Update(_storageBits, _piecesCountMask, _piecesCount);
            }
        }

        /// <summary>
        /// Mask to board field aligned from storageBits
        /// eg Pieces (packed with 0s) + MoveID (packed with 0s)  + Board (1s)
        /// </summary>
        public ulong BoardMask
        {
            get { return _boardMask; }
        }
        
        /// <summary>
        /// MoveID mask field aligned
        /// eg Pieces (packed with 0s) + MoveID (1s)  + Board (packed with 0s)
        /// </summary>
        public ulong MoveIDMask
        {
            get { return _moveIDMask; }
        }
        /// <summary>
        /// Pieces mask field aligned
        /// eg Pieces (1s) + MoveID (packed with 0s)  + Board (packed with 0s)
        /// </summary>
        public ulong PiecesCountMask
        {
            get { return _piecesCountMask; }
        }

        /// <summary>
        /// The storage bit packing board, moveId and pieces count fields
        /// eg Pieces (1s and 0s) + MoveID (1s and 0s)  + Board (1s and 0s)
        /// </summary>
        public ulong StorageBits
        {
            get { return _storageBits; }
            set {
                updateFields(value);
                _storageBits = value; 
            }
        }

        

        /// <summary>
        /// Ctor used to pack bits for storage in ulong
        /// </summary>
        public StorageBitPacker()
        {
            setMasks();
        }

        /// <summary>
        /// Sets pieces count field aligned 0 - 32
        /// Takes the field as input as a value and adds necessary packing
        /// </summary>
        /// <param name="count">count of pieces ushort</param>
        /// <returns>ulong field aligned</returns>
        public ulong SetPiecesCount(ushort count)
        {
            ulong value = 0;
            value |= ((ulong)count << PACKING_PRE_PIECESCOUNT);

            PiecesCount = value;
            return value;
        }

        

        /// <summary>
        /// Sets moveID nswe direction * 100 + index
        /// Takes the field as input as a value and adds necessary packing
        /// </summary>
        /// <param name="id">move id ushort</param>
        /// <returns>ulong field aligned</returns>
        public ulong SetMoveValueID(ushort id)
        {
            ulong value = 0;
            value |= ((ulong)id << PACKING_PRE_MOVEID);
            
            MoveID = value;
            return value;
        }

        /// <summary>
        /// Sets board as 33 piece positions
        /// Takes the field as input as a value and adds necessary packing
        /// </summary>
        /// <param name="board">ulong board</param>
        /// <returns>ulong field aligned</returns>
        public ulong SetBoard(ulong board)
        {
            Board = board;
            return board;
        }
        

        /// <summary>
        /// Setup masks as ulong field aligned (packed), called by the ctor
        /// </summary>
        void setMasks()
        {
            _boardMask = 0;
            for (int i = 0; i < NUMBER_BITS_GAME_POS; i++)
            {
                setBit(i, ref _boardMask);
            }

            _moveIDMask = 0;
            int maskRegionStart = NUMBER_BITS_GAME_POS;
            for (int i = maskRegionStart; i < maskRegionStart + NUMBER_BITS_MOVEID; i++)
            {
                setBit(i, ref _moveIDMask);
            }

            _piecesCountMask = 0;
            maskRegionStart = NUMBER_BITS_GAME_POS + NUMBER_BITS_MOVEID;
            for (int i = maskRegionStart; i < maskRegionStart + NUMBER_BITS_PIECESCOUNT; i++)
            {
                setBit(i, ref _piecesCountMask);
            }

        }

        public int UnpackPiecesCountFromStorageBits(ulong storageBits)
        {
            return (int)((storageBits & PiecesCountMask) >> PACKING_PRE_PIECESCOUNT);
        }

        public int UnpackMoveIdFromStorageBits(ulong storageBits)
        {
            return (int)((storageBits & MoveIDMask) >> PACKING_PRE_MOVEID);
        }

        /// <summary>
        /// Converts a sequence of chars of 1 and 0 s to ulong
        /// For the game board the string is read from left to right with bit 0 as left most
        /// char in string. 
        /// For moves Id and the Pieces count the strings are read right to left
        /// as these can be treated as ulong values
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public ulong ProcessString(string stringBits, StorageField field)
        {
            stringBits = StorageBitPacker.CleanString(stringBits);
            int index = -1;
            ulong bits = 0;
            char c;
            
            for (int i = stringBits.Length-1; i >=0 ; i--)
            {
                c = stringBits[i];
                if( c == '1')
                {
                    index++;
                    if (field == StorageField.All || field == StorageField.Board)
                    {
                        if (index < NUMBER_BITS_GAME_POS)
                            setBit(NUMBER_BITS_GAME_POS - 1 - index, ref bits);
                        else
                            setBit(index, ref bits);
                    }
                    else
                    {
                        setBit(index, ref bits);
                    }
                }
                if (c == '0')
                {
                    if (index >= END_SIGNIFICANT_BITS_INDEX) break;
                    index++;
                }

            }
            
            return bits;
        }

        /// <summary>
        /// Called when storagebits is updated, this sets the field values
        /// </summary>
        /// <param name="value"></param>
        private void updateFields(ulong value)
        {
            _piecesCount = getValue(value, _piecesCountMask);
            _moveID = getValue(value, _moveIDMask);
            _board = getValue(value, _boardMask);
        }

        public void Reset()
        {
            this.StorageBits = 0;
        }


        /// <summary>
        /// Update value using appropriate mask to update storagebits ulong
        /// </summary>
        /// <param name="storageBits">storageBits to update</param>
        /// <param name="mask">field mask</param>
        /// <param name="value">value to update as ulong field aligned</param>
        /// <returns></returns>
        public ulong Update(ulong storageBits, ulong mask, ulong value)
        {
            return (storageBits & ~mask) | value;
        }
        //is poss? -> ((board & mask) == premove)


        public ulong UpdateAll(ulong storageBits, ulong board, ulong moveID, ulong piecesCount)
        {
            storageBits = (storageBits & ~_boardMask) | board;
            storageBits = (storageBits & ~_moveIDMask) | moveID;
            storageBits = (storageBits & ~_piecesCountMask) | piecesCount;
            return storageBits;
        }

        public void UpdateAll(ref ulong storageBits, ulong board, ulong moveID, ulong piecesCount)
        {
            storageBits = (storageBits & ~_boardMask) | board;
            storageBits = (storageBits & ~_moveIDMask) | moveID;
            storageBits = (storageBits & ~_piecesCountMask) | piecesCount;
            
        }

        ulong getValue(ulong storageBits, ulong mask)
        {
            return (storageBits & mask);
        }
        
        void setBit(int index, ref UInt64 bits)
        {
            if (!validate(index)) return;
            bits |= (1ul << index);
        }

        bool testBit(int index, UInt64 board)
        {
            if (!validate(index)) return true;
            return (((board & (1ul << index)) != 0) ? true : false);
        }

        bool testBit(int index, UInt16 board)
        {
            //if (!validate(index)) return true;
            return (((board & (1ul << index)) != 0) ? true : false);
        }

        

        public void clearBit(int index, ref UInt64 bits)
        {
            if (!validate(index)) return;
            bits &= ~(1ul << index);
        }

        bool validate(int index)
        {
            return (index >= 0 && index < MAX_INDEX) ? true : false;
        }

        /// <summary>
        /// Converts string 1s and 0s dropping all other characters
        /// </summary>
        /// <param name="s"></param>
        /// <returns>cleaned string</returns>
        public static string CleanString(string s)
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

        /// <summary>
        /// Displays storagebits
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int index = -1;
            for (int i = END_SIGNIFICANT_BITS_INDEX-1; i >=0 ; i--)
            {
                index++;
                if (i == NUMBER_BITS_GAME_POS-1 || i == NUMBER_BITS_GAME_POS + NUMBER_BITS_MOVEID-1)
                {
                    sb.Append(" -+- ");
                }
                else
                {
                    if (i % 4 == 0) sb.Append(',');
                }

                if (i < NUMBER_BITS_GAME_POS)
                {
                    //board bits string left to right - bits right to left
                    index = NUMBER_BITS_GAME_POS - 1 - i;
                }
                else
                {
                    index = i;
                }
                
                if (testBit(index, _storageBits))
                {
                    sb.Append('1');
                }
                else
                {
                    sb.Append('0');
                }

                

            }
            return sb.ToString();
        }
    }
}
