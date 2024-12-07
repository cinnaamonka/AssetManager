using System.Collections.ObjectModel;
using System.IO;
using AssetManager.Models;

namespace AssetManager.AssetHelpers
{
    public class AssetHelpers
    {
    

        public enum AssetType
        {
            Image,
            Model,
            Obj,
            Document,
            Audio,
            Video,
            Other
        }

        public static List<string> AvailableFileFormats { get; } = new List<string>
    {
        ".png",
        ".jpg",
        ".jpeg",
        ".fbx",
        ".obj",
        ".mp3",
        ".wav",
        ".txt",
        ".docx",
        ".pdf",
        ".mp4"
    };

      
        public static string GetOpenFileDialogFilter()
        {
            var images = new[] { ".png", ".jpg", ".jpeg" };
            var models = new[] { ".fbx", ".obj" };
            var audio = new[] { ".mp3", ".wav" };
            var documents = new[] { ".txt", ".docx", ".pdf" };
            var videos = new[] { ".mp4" };

            string supportedFiles = string.Join(";", AvailableFileFormats.Select(ext => $"*{ext}"));
            string imageFilter = string.Join(";", images.Select(ext => $"*{ext}"));
            string modelFilter = string.Join(";", models.Select(ext => $"*{ext}"));
            string audioFilter = string.Join(";", audio.Select(ext => $"*{ext}"));
            string documentFilter = string.Join(";", documents.Select(ext => $"*{ext}"));
            string videoFilter = string.Join(";", videos.Select(ext => $"*{ext}"));

            return $"Supported Files|{supportedFiles}|" +
                   $"Images|{imageFilter}|" +
                   $"Models|{modelFilter}|" +
                   $"Audio|{audioFilter}|" +
                   $"Documents|{documentFilter}|" +
                   $"Videos|{videoFilter}|" +
                   "All Files|*.*";
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
                ".jpg" or "jpg" or ".jpeg" or "jpeg" => AssetType.Image,
                ".fbx" or "fbx" => AssetType.Model,
                ".obj" or "obj" => AssetType.Obj,
                ".mp3" or "mp3" => AssetType.Audio,
                ".wav" or "wav" => AssetType.Audio,
                ".txt" or "txt" => AssetType.Document,
                ".docx" or "docx" => AssetType.Document,
                ".pdf" or "pdf" => AssetType.Document,
                ".mp4" or "mp4" => AssetType.Video,

                _ => AssetType.Other
            };
        }

        static public bool IsSupportedFileType(string fileType)
        {

            string extension = Path.GetExtension(fileType).ToLower();
            return AvailableFileFormats.Contains(extension);
        }
        public static void AddVertexFileInfo(string fileName, Asset asset)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine("File does not exist");
                return;
            }

            foreach (var line in File.ReadAllLines(fileName))
            {
                if (line.StartsWith("v"))
                {
                    asset.Metadata.VertexCount++;
                }
                else if (line.StartsWith("f"))
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
            string resourcesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources"); 

            switch (fileExtension)
            {

                case "obj":
                case ".obj":
                    return Path.Combine(resourcesFolderPath, "OBJPlaceholder.png");

                case ".mp4":
                case ".wav":
                    return Path.Combine(resourcesFolderPath, "VideoPlaceholder.png");

                case ".txt":
                case ".docx":
                case ".pdf":
                    return Path.Combine(resourcesFolderPath, "TextPlaceholder.png");

                case ".mp3":
                    return Path.Combine(resourcesFolderPath, "WavePlaceholder.png");

                default:
                    return resourcesFolderPath;
            }
        }
    }
}
