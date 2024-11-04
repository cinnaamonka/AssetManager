using AssetManager.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AssetManager.ViewModels
{
    internal class ImageViewerVM : ObservableObject
    {
        private ImageSource _imagePath;
        public ImageSource ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public void SetImagePath(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                ImagePath = new BitmapImage(new Uri(imagePath));
            }
        }

        public RelayCommand CloseCommand { get; }


        public ImageViewerVM() 
        {
            CloseCommand = new RelayCommand(CloseWindow);
        }

        private void CloseWindow()
        {
            App.Current.Windows.OfType<ImageViewerWindow>().FirstOrDefault()?.Close();
        }
    }
}
