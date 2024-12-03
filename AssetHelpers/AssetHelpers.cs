using System.Collections.ObjectModel;
using System.IO;
using AssetManager.Models;

namespace AssetManager.AssetHelpers
{
    public class AssetHelpers
    {
        private static string _resourceFolder => "C:\\Users\\parni\\Desktop\\2024-2025\\semester5\\GraduationWork\\AssetManager\\Resources";

        public enum AssetType
        {
            Image,
            Model,
            Obj,

            Other
        }

        public enum Tags
        {
            Image,
            Model,
            Document,
            Video,
            Audio
        }
        static public string FindClosestColorName(Color inputColor)
        {
            // Define a list of known colors
            var knownColors = new Dictionary<string, Color>
        {
            { "Red", Color.Red },
            { "Green", Color.Green },
            { "Blue", Color.Blue },
            { "Yellow", Color.Yellow },
            { "Cyan", Color.Cyan },
            { "Magenta", Color.Magenta },
            { "White", Color.White },
            { "Black", Color.Black },
            { "Gray", Color.Gray },
            { "Orange", Color.Orange },
            { "Purple", Color.Purple },
            { "Pink", Color.Pink },
        };

            string closestColorName = null;
            double smallestDistance = double.MaxValue;

            foreach (var kvp in knownColors)
            {
                string name = kvp.Key;
                Color knownColor = kvp.Value;

                // Calculate Euclidean distance
                double distance = Math.Sqrt(
                    Math.Pow(inputColor.R - knownColor.R, 2) +
                    Math.Pow(inputColor.G - knownColor.G, 2) +
                    Math.Pow(inputColor.B - knownColor.B, 2)
                );

                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    closestColorName = name;
                }
            }

            return closestColorName;
        }

    public static AssetType GetAssetType(string extension)
        {
            return extension.ToLower() switch
            {
                ".png" or "png" => AssetType.Image,
                ".jpg" or "jpg" => AssetType.Image,
                ".fbx" or "fbx" => AssetType.Model,
                ".obj" or "obj" => AssetType.Obj,
                _ => AssetType.Other
            };
        }

        static public bool IsSupportedFileType(string fileType)
        {
            string[] supportedExtensions = { ".png", ".jpg", ".jpeg", ".fbx", ".obj" };
            string extension = Path.GetExtension(fileType).ToLower();
            return supportedExtensions.Contains(extension);
        }
        public static void AddVertexFileInfo(string fileName, Asset asset)
        {
            if(!File.Exists(fileName))
            {
                Console.WriteLine("File does not exist");
                return;
            }

            foreach(var line in File.ReadAllLines(fileName))
            {
                if(line.StartsWith("v"))
                {
                    asset.Metadata.VertexCount++;
                }
                else if(line.StartsWith("f"))
                {
                    asset.Metadata.FaceCount++;
                }
            
            }
        }

        public enum AvailableFormats
        {
            FBX,
            OBJ,
            PNG,
            JPG
        }

      

        public static System.Windows.Media.Color GenerateRandomColorRGB()
        {
            Random random = new Random();
            byte r = (byte)random.Next(128, 256);
            byte g = (byte)random.Next(128, 256);
            byte b = (byte)random.Next(128, 256);

            System.Windows.Media.Color brightColor = System.Windows.Media.Color.FromRgb(r, g, b);

            return brightColor;
        }

        public static List<string> AvailableColors { get; } = new List<string>
    {
        "Red", 
        "Green", 
        "Blue", 
        "Yellow",   
        "Orange",
        "Purple"
    };
        public static string GetPlaceholderPath(string extension)
        {
            string fileExtension = extension.ToLowerInvariant();

            switch (fileExtension)
            {

                case "obj":
                case ".obj":
                    return Path.Combine(_resourceFolder, "OBJPlaceholder.png");

                //case ".mp3":
                //case ".wav":
                //    return @"Placeholders\AudioPlaceholder.png";

                //case ".txt":
                //case ".docx":
                //case ".pdf":
                //    return @"Placeholders\DocumentPlaceholder.png"; 

                default:
                    return _resourceFolder;
            }
        }
    }
}
