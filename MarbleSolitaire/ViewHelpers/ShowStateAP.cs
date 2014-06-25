using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MarbleSolitaire.ViewHelpers
{
    public class ShowStateAP
    {
        public static readonly DependencyProperty ShowStateProperty;
        
        static ShowStateAP()
        {
            int defaultState = -9;
            
            /// <summary>
            /// ShowStateAP Attached Dependency Property
            /// </summary>
            ShowStateProperty = DependencyProperty.RegisterAttached(
                "ShowState", 
                typeof(int), 
                typeof(ShowStateAP),
                new PropertyMetadata(defaultState));
        }
            
        ///<summary>
        /// Gets the ShowStateAP property. This dependency property 
        /// indicates ....
        /// </summary>
        public static int GetShowState(DependencyObject d)
        {
            return (int)d.GetValue(ShowStateProperty);
        }

        /// <summary>
        /// Sets the ShowStateAP property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetShowState(DependencyObject d, int value)
        {
            d.SetValue(ShowStateProperty, value);
        }
    }
}

