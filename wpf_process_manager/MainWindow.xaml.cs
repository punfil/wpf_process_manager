using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using wpf_process_manager.ViewModels;

namespace wpf_process_manager
{
    public partial class MainWindow : Window
    {
        private ProcessManagerViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new ProcessManagerViewModel();

            DataContext = _viewModel;
        }
    }
}
