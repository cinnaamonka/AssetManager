using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;
using System.Windows.Media;
using static AssetManager.AssetHelpers.AssetHelpers;

namespace AssetManager.Models
{
    public class Asset
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }


        public string FileName { get; set; }
        public string FilePath { get; set; }
        public AssetType FileType { get; }


        public string RelativePath { get; set; }

        [NotMapped] public ImageSource PreviewImage { get; set; }

        public AssetMetadata Metadata { get; set; }

        public Asset() { }

        public Asset(string name, string filePath,int projectId, AssetType fileType, string relativePath = "", ImageSource preview = null)
        {
            
            FileName = name;
            FilePath = filePath;
            FileType = fileType;
            RelativePath = relativePath;
            PreviewImage = preview;
            ProjectId = projectId;

            Metadata = new AssetMetadata();
        }

      
    }

  
}
