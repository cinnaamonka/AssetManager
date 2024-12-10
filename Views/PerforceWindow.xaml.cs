using AssetManager.ViewModels;
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
using System.Windows.Shapes;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for PerforceWindow.xaml
    /// </summary>
    public partial class PerforceWindow : Window
    {
        public PerforceWindow()
        {
            InitializeComponent();

        
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is PerforceWindowVM viewModel)
            {
                viewModel.PerforceConfiguration.Password = PasswordBox.Password; 
            }
        }
    }
}
