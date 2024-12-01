using AssetManager.Models;
using AssetManager.ViewModels;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AssetManager.Views
{
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

        private void TagArea_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Tag)))
            {
                Tag droppedTag = e.Data.GetData(typeof(Tag)) as Tag;

                var viewModel = (OverviewPageVM)DataContext;

                if (viewModel.SelectedAsset != null)
                {
                    viewModel.AddAssetTag(droppedTag.Name);
                    (sender as Border).BorderBrush = System.Windows.Media.Brushes.Transparent;
                }
            }
        }

        private void TagArea_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Tag)))
            {
                // Change the appearance of the drop target
                (sender as Border).BorderBrush = System.Windows.Media.Brushes.LightGreen;
            }
        }

        private void TagArea_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            (sender as Border).BorderBrush = System.Windows.Media.Brushes.Transparent;
        }

        private void AssetArea_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                var viewModel = (OverviewPageVM)DataContext;

                foreach (var filePath in filePaths)
                {
                    viewModel.AddAsset(filePath);
                }

                (sender as System.Windows.Controls.ListBox).BorderBrush = System.Windows.Media.Brushes.Transparent;
            }
        }



        private void AssetArea_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(Tag)))
            {
                (sender as System.Windows.Controls.ListBox).BorderBrush = System.Windows.Media.Brushes.LightGreen;
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
        }


        private void AssetArea_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            (sender as System.Windows.Controls.ListBox).BorderBrush = System.Windows.Media.Brushes.Transparent;



        }

        private void FileNameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var viewModel = (OverviewPageVM)DataContext;

                viewModel.RenameAsset(viewModel.SelectedAsset);

                var parent = (sender as System.Windows.Controls.TextBox)?.Parent as UIElement;
                parent?.Focus();
                e.Handled = true;
            }
        }

        private void TagsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.ListBox listBox && listBox.SelectedItem != null)
            {
                var viewModel = DataContext as OverviewPageVM; 
                if (viewModel != null)
                {
                  //  viewModel.SelectedTag = listBox.SelectedItem as Tag;
                    viewModel.FilterAssetsByTag();
                }
            }
        }
    }
}
