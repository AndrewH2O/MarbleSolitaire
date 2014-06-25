
using MarbleSolitaireLib.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.Data
{
    public class DataLoader
    {
        /// <summary>
        /// Load EnumSolutionsDTO from file, the isError flag reports any issues.
        /// </summary>
        /// <param name="isError"></param>
        /// <returns></returns>
        public static EnumSolutionsDTO GetData(out bool isError)
        {
            isError = false;
            string fileNameInTest = "EnumDto.dat";
            EnumSolutionsDTO dto = null;
            if (!File.Exists(fileNameInTest))
            {
                isError = true;
                //throw new FileNotFoundException("could not find data file");
                //return null;
            }
            else 
            {
                dto = (EnumSolutionsDTO)SolverIO<IEnumSolutionsDTO>.RetrieveBinary();
            }

            return dto;
        }
    }
}
