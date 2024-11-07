using AssetManager.Models;
using System.IO;
using System.Text.Json;

namespace AssetManager.Services
{
    public class MetadataRepository
    {
       
        public MetadataRepository()
        {
           
        }

        public async Task SaveMetadataAsync(AssetMetadata metadata)
        {
            //var filePath = Path.Combine(_metadataFolder, $"{metadata.Name}.json");
            //var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
            //await File.WriteAllTextAsync(filePath, json);
        }

        public AssetMetadata LoadMetadata(Asset asset)
        {
            asset.Metadata = new AssetMetadata
            {
                Name = asset.FileName,
                FilePath = asset.FilePath,
                FileType = asset.FileType,
                Format = "Not defined",

            };

            if (asset.Metadata.FilePath != "")
            {
                FileInfo fileInfo = new FileInfo(asset.Metadata.FilePath);
                asset.Metadata.FileSize = Math.Round((fileInfo.Length / 1024.0), 0);
                asset.Metadata.Format = fileInfo.Extension;
                asset.Metadata.DateCreated = fileInfo.CreationTimeUtc;
                asset.Metadata.DateLastChanged = fileInfo.LastAccessTimeUtc;
               // asset.Metadata.Id = asset.Id;
            }
            else
            {
                asset.Metadata.FileSize = 0;
            }

            return asset.Metadata;
           
        }


    }
}
