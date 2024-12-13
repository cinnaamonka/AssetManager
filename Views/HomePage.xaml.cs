using AssetManager.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.DataContext is Project project)
            {

                var viewModel = DataContext as HomePageVM;
                viewModel.SelectedProject = project;

                if (viewModel?.RemoveProjectCommand.CanExecute(project) == true)
                {
                    viewModel.RemoveProjectCommand.Execute(project);
                }
            }

            e.Handled = true;
        }

        private void OpenPerforceConnection(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.DataContext is Project project)
            {
                var viewModel = DataContext as HomePageVM;
                viewModel.SelectedProject = project;
                viewModel.OpenVersionControl();

            }

            e.Handled = true;
        }
    }
}
