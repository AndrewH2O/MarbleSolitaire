using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MarbleSolitaire.ViewHelpers
{
    public class IntConverter:IValueConverter
    {
        
                //    "1,1,1," +    //6 - (0-2)
                //    "1,1,1," +    //13 - (3-5)
                //"1,1,1,1,1,1,1" +    //20 - (6-12)
                //"1,1,1,0,1,1,1" +    //27 - (13-19)
                //"1,1,1,1,1,1,1" +    //34 - (20-26)
                //    "1,1,1," +    //41 - (27-29)
                //    "1,1,1,";   
        int[] _mapper = new int[]{ 
            2,3,4, 
            9,10,11 ,
            14,15,16,17,18,19,20,
            21,22,23,24,25,26,27,
            28,29,30,31,32,33,34,
            37,38,39,
            44,45,46
        };
        
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is int)) return null;
            int index = (int)value;
            if (!(index >=0 && index < 33)) return 0;
            return _mapper[index];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
 	        throw new NotImplementedException();
        }
    }
    
}
