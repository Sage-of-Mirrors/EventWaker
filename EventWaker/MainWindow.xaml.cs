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
using EventWaker.ViewModel;
using Graph;

namespace EventWaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataViewModel mViewModel;

        public MainWindow()
        {
            mViewModel = new DataViewModel();
            DataContext = mViewModel;
            InitializeComponent();
        }

        private void NodeHost_Initialized(object sender, EventArgs e)
        {
            //viewModel.Graph.CompatibilityStrategy = new Graph.Compatibility.TagTypeCompatibility();
            mViewModel.Graph.AllowDrop = true;
            mViewModel.Graph.BackColor = System.Drawing.Color.FromArgb(255, 100, 100, 100);

            NodeHost.Child = mViewModel.Graph;
            NodeHost.AllowDrop = true;
        }
    }
}
