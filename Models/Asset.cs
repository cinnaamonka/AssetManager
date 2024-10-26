using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace AssetManager.Models
{
    public class Asset
    {
       // public string Id { get; set; }        
        public string FileName { get; set; }       
        public string FilePath { get; set; }  
        public string FileType { get; set; } 
        //public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }

        public AssetMetadata Metadata { get; set; }

        public ICommand OpenMetadataCommand { get; }

        public Asset(/*string id,*/ string name, string filePath, string fileType)
        {
           //Id = id;
            FileName = name;
            FilePath = filePath;
            FileType = fileType;
            //DateCreated = DateTime.Now;
            //DateModified = DateTime.Now;
            Metadata = new AssetMetadata();
            OpenMetadataCommand = new RelayCommand(OpenMetadata);
        }

        private void OpenMetadata()
        {
            // Raise an event or invoke a callback
            MetadataRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler MetadataRequested;
    }
}
