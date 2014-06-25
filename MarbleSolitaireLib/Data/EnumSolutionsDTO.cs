using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.Data
{
   
    public interface IEnumSolutionsDTO
    {

        bool LoadData(int piecesCount, ulong[] boardIndexes);
        bool LoadData(int piecesCount, List<ulong> boards);

        int PiecesCount { get; }

        [DataMember]
        ulong this[ulong piecesCount, ulong boardIndex] { get; }

        [DataMember]
        ulong[] this[ulong piecesCount] { get; }
    }
    
    
    [DataContract]
    [Serializable]
    public class EnumSolutionsDTO:IEnumSolutionsDTO
    {
        ulong[][] _array;

        List<ulong>[] _arrayOflists;

        [DataMember]
        public ulong this[ulong piecesCount, ulong boardIndex]
        {
            get { return _array[piecesCount][boardIndex]; }
        }

        [DataMember]
        public ulong[] this[ulong piecesCount]
        {
            get { return _array[piecesCount]; }
        }

        public int PiecesCount { get; set; }
        
        public EnumSolutionsDTO(int piecesCount)
        {
            _array = new ulong[piecesCount][];
            
            _arrayOflists = new List<ulong>[piecesCount];
            PiecesCount = piecesCount;
        }
        
        /// <summary>
        /// Accepts an array representing boards all with the same piecesCount. 
        /// Note piecesCount must be in range 1 to 32 inclusive.
        /// Any array of boards added overwrites existing board data.
        /// </summary>
        /// <param name="piecesCount"></param>
        /// <param name="boardIndexes"></param>
        public bool LoadData(int piecesCount, ulong[] boardIndexes)
        {
            if (!validate(piecesCount, boardIndexes))
            {
                return false;
            }
            else
            {
                _array[piecesCount - 1] = new ulong[boardIndexes.Length];
                for (int i = 0; i < boardIndexes.Length; i++)
                {
                    _array[piecesCount - 1][i] = boardIndexes[i];
                }
                return true;
            }
            
        }

        public bool LoadData(int piecesCount, List<ulong> boards)
        {
            if (!validate(piecesCount, boards))
            {
                return false;
            }
            else
            {
                _array[piecesCount - 1]  = boards.ToArray();
                return true;
            }

        }

        public void AppendData(int piecesCount, ulong board)
        {
            if (!validate(piecesCount)) return;
            if (_arrayOflists[piecesCount - 1] == null) 
                _arrayOflists[piecesCount - 1] = new List<ulong>();
            _arrayOflists[piecesCount - 1].Add(board);
            
        }

        public void AppendData(ulong piecesCount, ulong board)
        {
            if (!validate(piecesCount)) return;
            if (_arrayOflists[piecesCount - 1] == null) 
                _arrayOflists[piecesCount - 1] = new List<ulong>();

            _arrayOflists[piecesCount - 1].Add(board);
        }
        
        /// <summary>
        /// Generates an array internally and deletes the list
        /// Use this after appending data
        /// </summary>
        /// <returns></returns>
        public bool GenerateArrays()
        {
            
            if (_arrayOflists == null)return false;
            int count = _arrayOflists.Count();
            
            if(count==0||count!=_array.Length)
            {
                return false; 
            }
            else
            {
                Array.Clear(_array, 0, _array.Length);
                int length = _arrayOflists.Count();
                for (int i = 0; i < length; i++)
                {
                    _array[i] = _arrayOflists[i].ToArray();
                }

               

                return true;
            }
        }


        private bool validate(ulong piecesCount)
        {
            if (piecesCount > 0 && piecesCount < 33)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool validate(int piecesCount, ulong[] boardIndexes)
        {
            if (validate(piecesCount) && (boardIndexes != null && boardIndexes.Length > 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool validate(int piecesCount, List<ulong> boards)
        {
            if (validate(piecesCount) && (boards != null && boards.Count > 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool validate(int piecesCount)
        {
            if (piecesCount > 0 && piecesCount < 33)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        
    }
}
