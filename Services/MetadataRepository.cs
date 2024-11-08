using AssetManager.Models;
using Microsoft.EntityFrameworkCore;
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
           

            return asset.Metadata;


        }


    }
}
