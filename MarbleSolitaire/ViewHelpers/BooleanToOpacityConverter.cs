using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MarbleSolitaire.ViewHelpers
{
    public class BooleanToOpacityConverter:IValueConverter
    {
        double faded = 0.2;
        double full = 1.0;
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(!(value is bool)) return null;
            return (bool)value ? faded:full;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
