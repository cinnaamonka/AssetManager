using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AssetManager.Models;

namespace AssetManager.Repositories
{
    public class AssetRepository
    {
        private readonly int thumbnailSize = 120;
        public List<Asset> Assets { get; private set; } = new List<Asset>();

        public AssetRepository()
        {
        }

        public List<Asset> LoadAssetsFromUnityProject(string projectPath)
        {
            Assets.Clear();

            string assetsFolderPath = Path.Combine(projectPath, "Assets");

            if (Directory.Exists(assetsFolderPath))
            {
                string[] files = Directory.GetFiles(assetsFolderPath, "*.*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    // Exclude certain file types
                    if (!file.EndsWith(".meta") && !file.EndsWith(".asset") && !file.EndsWith(".uss") && !file.EndsWith(".cs"))
                    {
                        string result = file.Replace(" ", "");
                        string relativePath = result.Substring(assetsFolderPath.Length + 1);
                        string extension = Path.GetExtension(result).ToLower();

                        // Only include supported file types
                        if (extension == ".png" || extension == ".jpg" || extension == ".fbx")
                        {
                            
                            var asset = new Asset(
                                name: Path.GetFileName(result),
                                filePath: result,
                                fileType: extension,
                                relativePath: relativePath);

                            asset.PreviewImage = GenerateThumbnail(result, extension);
                            Assets.Add(asset);
                        }
                    }
                }
            }

            return Assets;
        }

        private ImageSource GenerateThumbnail(string filePath, string extension)
        {
            if (extension == ".png" || extension == ".jpg")
            {
                return LoadImageThumbnail(filePath);
            }
            else if (extension == ".fbx" )
            {
                string objFilePath = ConvertFbxToObj(filePath);

                // Step 2: Generate thumbnail for OBJ
                if (!string.IsNullOrEmpty(objFilePath) && File.Exists(objFilePath))
                {
                    GenerateObjThumbnail(objFilePath);

                    string filename = Path.ChangeExtension(objFilePath,".png");

                    return LoadImageThumbnail(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
                    
                }
            }
            else if(extension == ".prefab")
            {
                return GetPlaceholderThumbnail(); // Placeholder for 3D assets
            }
            return null;
        }
        private string ConvertFbxToObj(string fbxFilePath)
        {
            string objFilePath = Path.ChangeExtension(fbxFilePath, ".obj");
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools\\FBXToObjConverter", "FBXToObjConverter.exe");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {

                FileName = exePath,
                Arguments = $"\"{fbxFilePath}\" \"{objFilePath}\"", // Pass FBX and OBJ paths as arguments
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                bool test = File.Exists(objFilePath);
                if (process.ExitCode == 0 && test)
                {
                    return objFilePath;
                }
            }
            return null;
        }
        private string GenerateObjThumbnail(string objFilePath)
        {
            string thumbnailPath = Path.ChangeExtension(objFilePath, ".png");
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools\\ThumbnailsGenerator", "DirectX.exe");

            string outputFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempThumbnails");

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{objFilePath}\" \"{outputFolderPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    return thumbnailPath;
                }
            }
            return null;
         }

        private ImageSource LoadImageThumbnail(string filePath)
        {
            if(filePath != null)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.DecodePixelWidth = thumbnailSize;
                bitmap.EndInit();

                return bitmap;
            }
            return null;
        }

        private ImageSource GetPlaceholderThumbnail()
        {
            string placeholderPath = "path/to/placeholder.png";
            if (File.Exists(placeholderPath))
            {
                return new BitmapImage(new Uri(placeholderPath));
            }
            return null;
        }
    }
}
