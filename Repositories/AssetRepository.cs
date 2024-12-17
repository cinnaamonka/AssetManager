using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using AssetManager.AssetHelpers;
using AssetManager.Models;
using AssetManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using static AssetManager.AssetHelpers.AssetHelpers;

namespace AssetManager.Repositories
{
    public class AssetRepository
    {
        private readonly int thumbnailSize = 120;

        public AssetRepository() { }

        // MAIN FUNCTIONS
        public void SaveAsset(Asset asset, AppDbContext context)
        {
            var assets = GetAssets(context);

            if (!assets.Any(a => a.FileName == asset.FileName && a.FilePath == asset.FilePath && a.ProjectId == asset.ProjectId))
            {
                context.Assets.Add(asset);
                context.SaveChanges();
            }


        }
        public void AddAssets(List<Asset> assets, AppDbContext context)
        {
            foreach (var asset in assets)
            {
                SaveAsset(asset, context);
            }
        }
        public List<Asset> GetAssets(AppDbContext context)
        {
            return context.Assets.ToList();
        }
        public void RemoveAllAsssets(AppDbContext context)
        {
            var allAssets = context.Assets.ToList();
            context.Assets.RemoveRange(allAssets);
            context.SaveChanges();
        }
        public void DeleteAssetFromProject(Asset selectedAsset, AppDbContext context)
        {
            context.Remove(selectedAsset);
            context.SaveChanges();

            if (Path.Exists(selectedAsset.FilePath))
            {
                File.SetAttributes(selectedAsset.FilePath, FileAttributes.Normal);

                File.Delete(selectedAsset.FilePath);
            }

        }

        public void RenameAsset(Asset asset, AppDbContext context)
        {
            File.SetAttributes(asset.FilePath, FileAttributes.Normal);

            string directory = Path.GetDirectoryName(asset.FilePath);

            string newFilePath = Path.Combine(directory, asset.FileName);

            File.Move(asset.FilePath, newFilePath);

            asset.FilePath = newFilePath;

            UpdateAsset(asset, context);
        }
        public void UpdateAsset(Asset asset, AppDbContext context)
        {

            context.Assets.Update(asset);
            context.SaveChanges();

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
        public void ProcessAssetConversion(string selectedFromFormat, string selectedToFormat, Asset selectedAsset, AppDbContext context)
        {
            var conversionMap = new Dictionary<(string, string), Func<string, string>>
             {
                { ("FBX", "OBJ"), filePath => FileConverter.ConvertFbxToObj(filePath) },
                { ("PNG", "JPG"),FileConverter.ConvertPngToJpg },
                { ("JPG", "PNG"),FileConverter.ConvertJpgToPng },
                { ("OBJ", "FBX"),FileConverter.ConvertObjToFbx }
             };

            if (conversionMap.TryGetValue((selectedFromFormat, selectedToFormat), out var conversionFunc))
            {
                string convertedFilePath = conversionFunc(selectedAsset.FilePath);

                if (!string.IsNullOrEmpty(convertedFilePath))
                {
                    var asset = CreateAsset(convertedFilePath, context.Projects.FirstOrDefault(project => project.Id == selectedAsset.ProjectId), selectedToFormat, context);
                    SaveAsset(asset, context);
                }
            }
        }

        public Asset CreateAsset(string filePath, Project selectedProject, string targetFormat, AppDbContext context)
        {

            string fileName = Path.GetFileName(filePath);
            var fileInfo = new FileInfo(filePath);

            var asset = new Asset(
                name: fileName,
                filePath: filePath,
                projectId: selectedProject.Id,
                fileType: AssetHelpers.AssetHelpers.GetAssetType(targetFormat),
                relativePath: fileName
            );


            var metadata = new AssetMetadata
            {
                Name = fileName,
                FilePath = filePath,
                FileType = GetAssetType(targetFormat).ToString(),
                Format = fileInfo.Extension,
                FileSize = Math.Round((fileInfo.Length / 1024.0), 0),
                DateCreated = fileInfo.CreationTimeUtc,
                DateLastChanged = fileInfo.LastAccessTimeUtc,
                Author = String.Empty
            };

            asset.Metadata = metadata;

            asset.PreviewImagePath = GenerateThumbnail(ref asset, Path.GetExtension(filePath), selectedProject.Name);

            var existingTag = context.Tags.FirstOrDefault(t => t.Name == asset.FileType.ToString());


            if (existingTag == null)
            {
                existingTag = new Tag { Name = asset.FileType.ToString(), Color = GenerateRandomColorRGB().ToString() };
                context.Tags.Add(existingTag);

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

            }

            return asset;


        }
        public async Task<List<Asset>> LoadAssetsFromUnityProjectAsync(Project selectedProject, AppDbContext context,
            TagRepository tagRepository)
        {
            string assetsFolderPath = Path.Combine(selectedProject.Path, "Assets");


            var existingAssets = await GetAssetsByProjectPathAsync(selectedProject.Path, context);


            AddAssets(existingAssets, context);

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

                            string relativePath = file.Substring(assetsFolderPath.Length + 1);
                            string extension = Path.GetExtension(file).ToLower();

                            if (AssetHelpers.AssetHelpers.AvailableFileFormats.Any(format =>
                                format.Equals(extension, StringComparison.OrdinalIgnoreCase) &&
                                !existingAssets.Any(a => a.FilePath == file)))
                            {
                                var asset = CreateAsset(file, selectedProject, extension, context);


                                if (asset == null) continue;
                                SaveAsset(asset, context);

                                if (extension == ".png" || extension == ".jpg")
                                {
                                    string tagColor = AssetHelpers.ColorDetector.GetColor(asset.FilePath);

                                    if (tagColor != null)
                                    {
                                        var numbers = Regex.Matches(tagColor, @"\d+");

                                        int[] rgb = numbers.Cast<Match>()
                                       .Select(m => int.Parse(m.Value))
                                       .ToArray();


                                        if (rgb.Length == 3)
                                        {
                                            System.Drawing.Color inputColor = System.Drawing.Color.FromArgb(rgb[0], rgb[1], rgb[2]);

                                            string closestColorName = FindClosestColorName(inputColor);

                                            await tagRepository.AddAssetTagAsync(asset.Id, closestColorName, closestColorName);
                                        }
                                    }
                                }
                            }
                        }
                    }

                });
            }
            return GetAssets(context);
        }


        public async Task SaveAssetAsync(Asset asset, AppDbContext context)
        {
            context.Assets.Add(asset);

            await context.SaveChangesAsync();
        }

        public string GenerateThumbnail(ref Asset asset, string extension, string currentProjectName)
        {
            switch (extension.ToLower())
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                    {
                        string outputFolderPath = Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            "Projects",
                            currentProjectName,
                            "Thumbnails",
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

                case ".fbx":
                    {
                        string objFilePath = FileConverter.ConvertFbxToObj(
                            asset.FilePath,
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                            "Projects",
                            currentProjectName,
                            "ObjFiles")
                        );

                        AddVertexFileInfo(objFilePath, asset);

                        if (!string.IsNullOrEmpty(objFilePath) && File.Exists(objFilePath))
                        {
                            string filename = GeneratePngThumbnail(objFilePath, currentProjectName);

                            return LoadAndSaveImageThumbnail(
                                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename),
                                thumbnailSize, currentProjectName
                            );
                        }

                        return null;
                    }

                case ".obj" or ".mp3" or ".mp4" or ".txt" or ".pdf" or ".wav" or ".docx":
                    {
                        return AssetHelpers.AssetHelpers.GetPlaceholderPath(extension);
                    }

                default:
                    return null;
            }
        }


        private string GeneratePngThumbnail(string objFilePath, string selectedProjectName)
        {

            string originalThumbnailPath = Path.ChangeExtension(objFilePath, ".png");
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools\\ThumbnailsGenerator", "DirectX.exe");


            string outputFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Projects",
                            selectedProjectName,
                            "Thumbnails");

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


        private string LoadAndSaveImageThumbnail(string filePath, int thumbnailSize, string selectedProjectName, string outputFilePath = "")
        {
            // Validate input
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new ArgumentException("The provided file path is null, empty, or does not exist.");
            }

            // Determine output path
            if (string.IsNullOrEmpty(outputFilePath))
            {
                string directory = Path.Combine(Path.GetDirectoryName(filePath), selectedProjectName, "Thumbnails");
                Directory.CreateDirectory(directory); // Ensure the thumbnails folder exists
                outputFilePath = Path.Combine(directory, Path.GetFileName(filePath));
            }

            // Skip processing if thumbnail already exists
            if (File.Exists(outputFilePath))
            {
                return outputFilePath;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.DecodePixelWidth = thumbnailSize;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                bitmap.EndInit();
                bitmap.Freeze();

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                using (var fileStream = new FileStream(outputFilePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                return outputFilePath;

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error creating thumbnail for {filePath}: {ex.Message}");
                throw;
            }
        }
    }
}
