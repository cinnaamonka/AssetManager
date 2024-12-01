using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AssetManager.ViewModels;

namespace AssetManager.Views
{
    public partial class TextViewerWindow : Window
    {
        public TextViewerWindow(string imagePath)
        {
            InitializeComponent();
            var viewModel = new TextViewerVM();
            viewModel.FileContent = imagePath;
            DataContext = viewModel;
        }
    }
}
