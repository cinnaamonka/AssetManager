using AssetManager.Models;
using AssetManager.ViewModels;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void TagsListBox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var listBox = sender as System.Windows.Controls.ListBox;
                var selectedItem = listBox?.SelectedItem;
                if (selectedItem != null)
                {
                    DragDrop.DoDragDrop(listBox, selectedItem, System.Windows.DragDropEffects.Copy);
                }
            }
        }

        private void AssetArea_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Tag)))
            {
                Tag droppedTag = e.Data.GetData(typeof(Tag)) as Tag;

                var viewModel = (OverviewPageVM)DataContext;

                if (viewModel.SelectedAsset != null)
                {
                    viewModel.AddAssetTag(droppedTag.Name);
                }
            }
        }

        private void AssetArea_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Tag)))
            {
                // Change the appearance of the drop target
                (sender as Border).Background = System.Windows.Media.Brushes.LightGreen;
            }
        }

        private void AssetArea_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            (sender as Border).Background = System.Windows.Media.Brushes.Transparent;
        }

    }
}
