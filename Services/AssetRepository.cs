using AssetManager.Models;
using Newtonsoft.Json;
using System.IO;


namespace AssetManager.Services
{
    internal class AssetRepository
    {
        private string _assetsFolderPath = "Assets";
       
        //we dont load assets into a variable because it is usable only for small scope projects
        public List<Asset> LoadAssets()
        {
            var assets = new List<Asset>();
            var files = Directory.GetFiles(_assetsFolderPath);

            foreach (var file in files)
            {
                var asset = new Asset(Path.GetFileName(file), file, DetermineAssetType(file));

                assets.Add(asset);
            }

            return assets;
        }

        private string DetermineAssetType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".png":
                case ".jpg":
                    return "Texture";
                case ".fbx":
                    return "3D Model";
                default:
                    return "Unknown";
            }
        }

        public void SaveAssetMetadata(Asset asset)
        {
            var metadataFile = Path.Combine(asset.FilePath, "metadata.json");
            var json = JsonConvert.SerializeObject(asset.Metadata);
            File.WriteAllText(metadataFile, json);
        }

    }
}
