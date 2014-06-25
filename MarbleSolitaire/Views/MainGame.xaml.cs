using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MarbleSolitaire.Views
{
    /// <summary>
    /// Interaction logic for MainGame.xaml
    /// </summary>
    public partial class MainGame : Window
    {
        public MainGame()
        {
            InitializeComponent();
            
        }

        private void CmdDragMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CmdClose(object sender, RoutedEventArgs e)
        {
            //call clean up on vms
            
            Close();
        }

        

        
    }
}
