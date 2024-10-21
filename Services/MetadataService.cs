using AssetManager.Models;
using System.IO;
using System.Text.Json;

namespace AssetManager.Services
{
    public class MetadataService
    {
        private const string _metadataFolder = "Metadata";

        public MetadataService()
        {
            if (!Directory.Exists(_metadataFolder))
            {
                Directory.CreateDirectory(_metadataFolder);
            }
        }

        public async Task SaveMetadataAsync(AssetMetadata metadata)
        {
            var filePath = Path.Combine(_metadataFolder, $"{metadata.Name}.json");
            var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<AssetMetadata> LoadMetadataAsync(string fileName)
        {
            var filePath = Path.Combine(_metadataFolder, $"{fileName}.json"); 
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<AssetMetadata>(json);
            }
            return null;
        }
    }
}
