using AssetManager.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

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

        private string _objFileContent;
        public string ObjFileContent
        {
            get => _objFileContent;
            set => SetProperty(ref _objFileContent, value);
        }


        public void SetImagePath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            string extension = System.IO.Path.GetExtension(imagePath).ToLower();

            if (extension == ".png" || extension == ".jpg")
            {
                ImagePath = new BitmapImage(new Uri(imagePath));
            }
            else if (extension == ".obj")
            {
                LoadObjFile(imagePath);
            }
            else
            {
                throw new InvalidOperationException("Unsupported file type.");
            }
        }
        private void LoadObjFile(string objFilePath)
        {
            try
            {
                string fileContent = System.IO.File.ReadAllText(objFilePath);

                ObjFileContent = fileContent; 
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error reading the OBJ file.", ex);
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
