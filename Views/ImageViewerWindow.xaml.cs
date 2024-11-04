using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AssetManager.ViewModels;

namespace AssetManager.Views
{
    public partial class ImageViewerWindow : Window
    {
        public ImageViewerWindow(string imagePath)
        {
            InitializeComponent();
            var viewModel = new ImageViewerVM();
            viewModel.SetImagePath(imagePath);
            DataContext = viewModel;
        }
    }
}
