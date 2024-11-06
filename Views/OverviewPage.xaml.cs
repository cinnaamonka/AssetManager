using AssetManager.Models;
using AssetManager.ViewModels;

using System.Windows;
using System.Windows.Controls;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class OverviewPage : Page
    {
        public OverviewPage()
        {
            InitializeComponent();
        }

        private void MetadataButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.DataContext is Asset asset)
            {
                // Get the ViewModel from the DataContext
                var viewModel = (OverviewPageVM)DataContext;

                // Set the SelectedAsset in the ViewModel to the clicked asset
                viewModel.SelectedAsset = asset;
            }
        }
    }
}
