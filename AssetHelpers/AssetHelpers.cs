
namespace AssetManager.AssetHelpers
{
    public class AssetHelpers
    {
        public enum AssetType
        {
            Image,
            Model,
            Other
        }

        public static AssetType GetAssetType(string extension)
        {
            return extension.ToLower() switch
            {
                ".png" => AssetType.Image,
                ".jpg" => AssetType.Image,
                ".fbx" => AssetType.Model,
                _ => AssetType.Other
            };
        }

    }
}
