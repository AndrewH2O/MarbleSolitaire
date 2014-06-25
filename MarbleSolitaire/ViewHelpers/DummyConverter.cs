
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MarbleSolitaire.ViewHelpers
{
    [ValueConversion(typeof(int), typeof(int))]
    public class DummyConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debug.WriteLine("DummyConverter-from object:" + value);
            Debug.WriteLine("DummyConverter-from param:" + parameter);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
            
            //uncomment if binding engine keeps returning -1 when debugging
            //if ((int)value != -1)
            //    return value;
            //else
            //    return DependencyProperty.UnsetValue;//binding engine kept returning unselected value
            
        }
    }
}
