using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AssetManager.Models
{
    public class Asset
    {
       // public string Id { get; set; }        
        public string FileName { get; set; }       
        public string FilePath { get; set; }  
        public string FileType { get; set; }
        public string RelativePath { get; set; }

        public ImageSource PreviewImage { get; private set; }

        //public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }

        public AssetMetadata Metadata { get; set; }

        public ICommand OpenMetadataCommand { get; }

        public Asset(string name, string filePath, string fileType, string relativePath = "")
        {
            //Id = id;
            FileName = name;
            FilePath = filePath;
            FileType = fileType;
            //DateCreated = DateTime.Now;
            //DateModified = DateTime.Now;
            Metadata = new AssetMetadata();
            OpenMetadataCommand = new RelayCommand(OpenMetadata);
            RelativePath = relativePath;

            LoadThumbnail();
        }

        private void OpenMetadata()
        {
            // Raise an event or invoke a callback
            MetadataRequested?.Invoke(this, EventArgs.Empty);
        }

        private void LoadThumbnail()
        {
            if (System.IO.File.Exists(FilePath))
            {
               
                string extension = System.IO.Path.GetExtension(FilePath)?.ToLower();
                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(FilePath);
                    bitmap.DecodePixelWidth = 120; 
                    bitmap.EndInit();

                    PreviewImage = bitmap;
                }
                else
                {
                    
                    PreviewImage = null;
                }
            }
        }

        public event EventHandler MetadataRequested;
    }
}
