using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AssetManager.Models;
using static AssetManager.AssetHelpers.AssetHelpers;

namespace AssetManager.Repositories
{
    public class AssetRepository
    {
        private readonly int thumbnailSize = 120;
        public List<Asset> Assets { get; private set; } = new List<Asset>();

        public AssetRepository()
        {
        }

        public async Task<List<Asset>> LoadAssetsFromUnityProjectAsync(string projectPath)
        {
            Assets.Clear();

            string assetsFolderPath = Path.Combine(projectPath, "Assets");

            if (Directory.Exists(assetsFolderPath))
            {
                string[] files = Directory.GetFiles(assetsFolderPath, "*.*", SearchOption.AllDirectories);

                // Run the asset processing on a background thread
                await Task.Run(() =>
                {
                    foreach (string file in files)
                    {
                        if (!file.EndsWith(".meta") && !file.EndsWith(".asset") && !file.EndsWith(".uss") && !file.EndsWith(".cs"))
                        {
                            string result = file.Replace(" ", "");
                            string relativePath = result.Substring(assetsFolderPath.Length + 1);
                            string extension = Path.GetExtension(result).ToLower();

                            if (extension == ".png" || extension == ".jpg" || extension == ".fbx")
                            {
                                var assetType = GetAssetType(extension);

                                var asset = new Asset(
                                    name: Path.GetFileName(result),
                                    filePath: result,
                                    fileType: assetType,
                                    relativePath: relativePath);

                                // Use Dispatcher to update the UI safely
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    asset.PreviewImage = GenerateThumbnail(result, extension);
                                    Assets.Add(asset);
                                });
                            }
                        }
                    }
                });
            }

            return Assets;
        }


        private ImageSource GenerateThumbnail(string filePath, string extension)
        {
            if (extension == ".png" || extension == ".jpg")
            {
                if(File.Exists(filePath))
                {
                    return LoadImageThumbnail(filePath);
                }
                return null;
               
            }
            else if (extension == ".fbx" )
            {
                string objFilePath = ConvertFbxToObj(filePath);

                if (!string.IsNullOrEmpty(objFilePath) && File.Exists(objFilePath))
                {
                    string filename = GeneratePngThumbnail(objFilePath);

                    return LoadImageThumbnail(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
                    
                }
            }
            else if(extension == ".prefab")
            {
                return GetPlaceholderThumbnail();
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
                Arguments = $"\"{fbxFilePath}\" \"{objFilePath}\"",
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
        private string GeneratePngThumbnail(string objFilePath)
        {
            // Define the target path for the PNG thumbnail in the same directory as the OBJ file
            string originalThumbnailPath = Path.ChangeExtension(objFilePath, ".png");
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools\\ThumbnailsGenerator", "DirectX.exe");

            // Define the temporary output folder for the thumbnail
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
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    // Construct the new path in the TempThumbnails folder
                    string tempThumbnailPath = Path.Combine(outputFolderPath, Path.GetFileName(originalThumbnailPath));

                    // If the file already exists, delete it to avoid IOException
                    if (File.Exists(tempThumbnailPath))
                    {
                        File.Delete(tempThumbnailPath);
                    }

                    // Move the generated file from the original location to the TempThumbnails folder
                    File.Move(originalThumbnailPath, tempThumbnailPath);

                    // Return the final path of the thumbnail in the TempThumbnails directory
                    return tempThumbnailPath;
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
