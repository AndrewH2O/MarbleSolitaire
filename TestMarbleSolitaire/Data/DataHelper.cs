using MarbleSolitaireLib.Data;
using MarbleSolitaireLib.GameSolver;
using MarbleSolitaireLib.Helpers;
using MarbleSolitaireModelLib.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMarbleSolitaire.Helpers;

namespace TestMarbleSolitaire.Data
{
    public class DataHelper
    {

        public static void SaveBinary<T> (T o, string fileName)
        {
            SolverIO<T>.SaveBinary(o, fileName);
        }
        
        
        /// <summary>
        /// Load EnumSolutionsDTO from file
        /// </summary>
        /// <param name="isError">error if file does not exist</param>
        /// <returns>EnumSolutionsDTO</returns>
        public static T GetOrCreateBinary<T>(string fileName,  out bool isError)
        {
            isError = false;
            
            T dto = default(T);
            if (!File.Exists(SolverIO<int>.GetBaseDirectory()+fileName))
            {
                dto = CreateDataFile<T>(fileName, out isError);
                if (dto==null) isError = true;
            }
            else
            {
                dto = SolverIO<T>.RetrieveBinary(fileName);
            }

            return dto;
        }

        public static T CreateDataFile<T>(string fileName, out bool isFailedToCreate)
        {
            isFailedToCreate = false;
            T dto = default(T);
            if (fileName == "EnumDto.dat")
            {
                CreateEnumDto();
                if (!File.Exists(SolverIO<int>.GetBaseDirectory() + fileName))
                {
                    isFailedToCreate = true;
                }
                else
                {
                    dto = SolverIO<T>.RetrieveBinary(fileName);
                }
            }
            
            return dto;
        }

        

        private static void CreateEnumDto()
        {
            //arrange
            SolverEnum solver = new SolverEnum(BoardFactory.GetSquareBoard());

            List<int> getStart = new List<int>() 
            { 
                0,0,1,1,1,0,0,//6 - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            };

            List<int> getWin = new List<int>()
            { 
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };

            solver.LoadBoard(getStart, LoadState.Start);
            solver.LoadBoard(getWin, LoadState.Win);
            //act
            solver.Solve(SaveState.ToBinary, "EnumDto.dat");
        }
    }
}
