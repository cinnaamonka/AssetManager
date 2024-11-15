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

        public string PreviewImagePath{ get; set; }

        public AssetMetadata Metadata { get; set; }

        public List<Tag> Tags { get; set; }

        public Asset() { }

        public Asset(string name, string filePath,int projectId, AssetType fileType, string relativePath = "", string previewImagePath = null)
        {
            
            FileName = name;
            FilePath = filePath;
            FileType = fileType;
            RelativePath = relativePath;
            PreviewImagePath = previewImagePath;
            ProjectId = projectId;

            Metadata = new AssetMetadata();
            Tags = new List<Tag>();
        }

      
    }

  
}
