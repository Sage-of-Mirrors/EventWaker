using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EventWaker.EventList;

namespace EventWaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MapEventList list = new MapEventList(@"D:\SZS Tools\EventList Test\test.dat");
            list.Write(@"D:\SZS Tools\EventList Test\test.dat");
            InitializeComponent();
        }
    }
}
