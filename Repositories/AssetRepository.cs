using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using AssetManager.Models;
using Microsoft.EntityFrameworkCore;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
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
        public async Task<List<Asset>> GetAssetsByProjectPathAsync(string projectPath, AppDbContext context)
        {

            var connection = context.Database.GetDbConnection();
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Assets';";
                var result = command.ExecuteScalar();

                if (result == null)
                {
                    throw new InvalidOperationException("The Assets table does not exist in the database.");
                }
            }

            return await context.Assets
                .Where(a => a.FilePath.StartsWith(projectPath))
                .Include(a => a.Metadata)
                .Include(a => a.AssetTags)
                .ToListAsync();
        }


        public async Task<List<Asset>> LoadAssetsFromUnityProjectAsync(string projectPath, AppDbContext context, int currentProjectId)
        {
            Assets.Clear();

            string assetsFolderPath = Path.Combine(projectPath, "Assets");


            var existingAssets = await GetAssetsByProjectPathAsync(projectPath, context);
            Assets.AddRange(existingAssets);

            if (existingAssets != null && existingAssets.Count > 0)
            {
                return existingAssets;
            }
            if (Directory.Exists(assetsFolderPath))
            {
                string[] files = Directory.GetFiles(assetsFolderPath, "*.*", SearchOption.AllDirectories);


                await Task.Run(async () =>
                {
                    foreach (string file in files)
                    {
                        if (!file.EndsWith(".meta") && !file.EndsWith(".asset") && !file.EndsWith(".uss") && !file.EndsWith(".cs"))
                        {
                            string result = file.Replace(" ", "");
                            string relativePath = result.Substring(assetsFolderPath.Length + 1);
                            string extension = Path.GetExtension(result).ToLower();

                            if ((extension == ".png" || extension == ".jpg" || extension == ".fbx") &&
                                !existingAssets.Any(a => a.FilePath == result))
                            {
                                var assetType = GetAssetType(extension);

                                var asset = new Asset(

                                    name: Path.GetFileName(result),
                                    filePath: result,
                                    projectId: currentProjectId,
                                    fileType: assetType,
                                    relativePath: relativePath

                                    );

                                asset.Metadata = new AssetMetadata
                                {
                                    Name = asset.FileName,
                                    FilePath = asset.FilePath,
                                    FileType = asset.FileType,
                                    Format = "Not defined",
                                };

                                if (!string.IsNullOrEmpty(asset.Metadata.FilePath))
                                {
                                    FileInfo fileInfo = new FileInfo(asset.Metadata.FilePath);
                                    asset.Metadata.FileSize = Math.Round((fileInfo.Length / 1024.0), 0);
                                    asset.Metadata.Format = fileInfo.Extension;
                                    asset.Metadata.DateCreated = fileInfo.CreationTimeUtc;
                                    asset.Metadata.DateLastChanged = fileInfo.LastAccessTimeUtc;

                                }
                                else
                                {
                                    asset.Metadata.FileSize = 0;
                                }

                               
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    asset.PreviewImagePath = GenerateThumbnail(asset, extension);
                                    Assets.Add(asset);
                                });


                                await SaveAssetAsync(asset, context);



                                var existingTag = context.Tags.FirstOrDefault(t => t.Name == asset.FileType.ToString());


                                if (existingTag == null)
                                {
                                    existingTag = new Tag { Name = asset.FileType.ToString(),Color = GenerateRandomColorRGB().ToString() };
                                    context.Tags.Add(existingTag);
                                    await context.SaveChangesAsync();

                                }

                                bool assetTagExists = asset.AssetTags.Any(at => at.TagId == existingTag.Id);

                                if (!assetTagExists)
                                {
                                    var assetTag = new AssetTag
                                    {
                                        Asset = asset,
                                        Tag = existingTag
                                    };

                                    asset.AssetTags.Add(assetTag);
                                    context.AssetTags.Add(assetTag);
                    
                                    await context.SaveChangesAsync();
                                   
                                }
                            }
                        }
                    }
                });
            }

            return Assets;
        }


        public async Task SaveAssetAsync(Asset asset, AppDbContext context)
        {
            context.Assets.Add(asset);

            await context.SaveChangesAsync();
        }

        public string GenerateThumbnail(Asset asset, string extension)
        {
            if (extension == ".png" || extension == ".jpg")
            {
                string outputFolderPath = Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            "TempThumbnails",
                            Path.GetFileNameWithoutExtension(asset.FilePath) + "_thumbnail.png"
 );

                if (File.Exists(outputFolderPath))
                {
                    return outputFolderPath;
                }

                if (File.Exists(asset.FilePath))
                {
                    return LoadAndSaveImageThumbnail(asset.FilePath, thumbnailSize, outputFolderPath);
                }
                return null;

            }
            else if (extension == ".fbx")
            {
                string objFilePath = ConvertFbxToObj(asset.FilePath);

                AddVertexFileInfo(objFilePath, asset);

                if (!string.IsNullOrEmpty(objFilePath) && File.Exists(objFilePath))
                {
                    string filename = GeneratePngThumbnail(objFilePath);


                    return LoadAndSaveImageThumbnail(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename), thumbnailSize);

                }
            }
            else if (extension == ".prefab")
            {
                return GetPlaceholderThumbnail();
            }
            return null;
        }
        public string ConvertFbxToObj(string fbxFilePath)
        {
            string outputFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempObjFiles");

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools\\FBXToObjConverter", "FBXToObjConverter.exe");

             ProcessStartInfo startInfo = new ProcessStartInfo
            {

                FileName = exePath,
                Arguments = $"\"{fbxFilePath}\" \"{outputFolderPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                if (process != null)
                {
                    process.WaitForExit();

                    if (process.ExitCode == 0 && Path.Exists(Path.Combine(outputFolderPath, Path.ChangeExtension(
                        Path.GetFileName(fbxFilePath), ".obj"))))
                    {
                        return Path.Combine(outputFolderPath, Path.ChangeExtension(
                        Path.GetFileName(fbxFilePath), ".obj"));
                    }
                }
            }
            return null;
        }
        public string ConvertPngToJpg(string pngFilePath)
        {
            
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools\\PngToJpgConverter", "PngToJpgConverter.exe");
            string outputFilePath = Path.ChangeExtension(pngFilePath, ".jpg");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{pngFilePath}\" \"{outputFilePath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                if (process != null)
                {
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        return outputFilePath;
                    }
                }
            }
            return null;
        }
        private string GeneratePngThumbnail(string objFilePath)
        {

            string originalThumbnailPath = Path.ChangeExtension(objFilePath, ".png");
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
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            using (Process process = Process.Start(startInfo))
            {
                if (process != null)
                {
                    process.WaitForExit();
                    if (process.ExitCode == 0)
                    {

                        string tempThumbnailPath = Path.Combine(outputFolderPath, Path.GetFileName(originalThumbnailPath));


                        if (File.Exists(tempThumbnailPath))
                        {
                            File.Delete(tempThumbnailPath);
                        }


                        File.Move(originalThumbnailPath, tempThumbnailPath);


                        return tempThumbnailPath;
                    }
                }
            }
            return null;
        }


        private string LoadAndSaveImageThumbnail(string filePath, int thumbnailSize, string outputFilePath = "")
        {
            if (outputFilePath == string.Empty)
            {
                outputFilePath = filePath;
            }

            if (File.Exists(outputFilePath))
            {
                return outputFilePath;
            }

            if (filePath != null)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.DecodePixelWidth = thumbnailSize;
                bitmap.EndInit();


                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));


                using (var fileStream = new FileStream(outputFilePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }


                return outputFilePath;
            }

            return null;
        }

        private string GetPlaceholderThumbnail()
        {
            string placeholderPath = "path/to/placeholder.png";

            if (!File.Exists(placeholderPath))
            {
                BitmapImage placeholderImage = new BitmapImage();
                placeholderImage.BeginInit();
                placeholderImage.UriSource = new Uri("pack://application:,,,/Resources/default_placeholder.png"); // Replace with your actual default image path if embedded in resources
                placeholderImage.CacheOption = BitmapCacheOption.OnLoad;
                placeholderImage.EndInit();

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(placeholderImage));

                using (FileStream fileStream = new FileStream(placeholderPath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }

            return placeholderPath;
        }

    }
}
