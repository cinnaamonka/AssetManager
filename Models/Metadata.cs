using static AssetManager.AssetHelpers.AssetHelpers;

namespace AssetManager.Models
{
    public class AssetMetadata
    { 
        public string Name { get; set; }
        public string FilePath { get; set; }
        public AssetType FileType { get; set; } // e.g., "Texture", "3D Model", "Audio"
        public long FileSize { get; set; }
        public string Format { get; set; }
     

        public AssetMetadata()
        {
            Name = "Not defined";
            FilePath = "Not defined";
            FileType = AssetType.Other;
            FileSize = 0;
            Format = "Not defined";
        }

      
    }
}
