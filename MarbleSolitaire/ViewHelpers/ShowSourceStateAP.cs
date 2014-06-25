using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MarbleSolitaire.ViewHelpers
{
    public class ShowSourceStateAP
    {
        public static readonly DependencyProperty ShowSourceStateProperty;

        static ShowSourceStateAP()
        {
            bool defaultState = false;
            
            /// <summary>
            /// ShowStateAP Attached Dependency Property
            /// </summary>
            ShowSourceStateProperty = DependencyProperty.RegisterAttached(
                "ShowSourceState", 
                typeof(bool), 
                typeof(ShowSourceStateAP),
                new PropertyMetadata(defaultState));
        }
            
        ///<summary>
        /// Gets the ShowStateAP property. This dependency property 
        /// indicates ....
        /// </summary>
        public static bool GetShowSourceState(DependencyObject d)
        {
            return (bool)d.GetValue(ShowSourceStateProperty);
        }

        /// <summary>
        /// Sets the ShowStateAP property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetShowSourceState(DependencyObject d, bool value)
        {
            d.SetValue(ShowSourceStateProperty, value);
        }
    }
}
