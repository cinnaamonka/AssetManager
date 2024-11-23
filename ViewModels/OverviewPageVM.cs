using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AssetManager.Models;
using System.Text.RegularExpressions;
using AssetManager.Repositories;
using AssetManager.Views;
using System.Windows.Input;
using System.IO;

namespace AssetManager.ViewModels
{
    public class OverviewPageVM : ObservableObject
    {
        public string RemoveIconPath => "C:\\Users\\parni\\Desktop\\2024-2025\\semester5\\GraduationWork\\AssetManager\\Resources\\minus.png";

        private List<Asset> _assets;
        private List<Asset> _filteredAssets;
        private Asset _selectedAsset;
        private List<Tag> _tags;


        public List<Asset> Assets
        {
            get { return _assets; }
            set
            {
                _assets = value;
                OnPropertyChanged(nameof(Assets));
            }
        }

        public List<Tag> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }
        public Asset SelectedAsset
        {
            get
            {
                return _selectedAsset;
            }
            set
            {
                _selectedAsset = value;
                OnPropertyChanged(nameof(SelectedAsset));
            }
        }

        public List<Asset> FilteredAssets
        {
            get { return _filteredAssets; }
            set
            {
                _filteredAssets = value;
                OnPropertyChanged(nameof(FilteredAssets));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ExecuteSearch(SearchText);
            }
        }

        public RelayCommand<string> SearchCommand { get; set; }
        public RelayCommand OpenMetadataCommand { get; set; }
        public RelayCommand OpenHomePageCommand { get; set; }

        public RelayCommand OpenFullImageCommand { get; set; }
        public RelayCommand ClearSelectionCommand { get; }

        public RelayCommand OpenMetadataFileCommand { get; }
        public RelayCommand RemoveAllTagsCommand { get; set; }
        public RelayCommand AddTagCommand { get; set; }
        public RelayCommand AddAssetTagCommand { get; set; }
        public ICommand RemoveTagCommand { get; set; }
        public ICommand RemoveAssetTagCommand { get; set; }
        public RelayCommand ConvertCommand { get; set; }

        public MainPageVM MainPageVM { get; }

        private AssetRepository _assetRepository;
        private TagRepository _tagRepository;



        private string _newTagName;

        public string NewTagName
        {
            get { return _newTagName; }
            set { _newTagName = value; }
        }

        private string _newAssetTagName;
        public string NewAssetTagName
        {
            get { return _newAssetTagName; }
            set { _newAssetTagName = value; }
        }

        private Tag _selectedTag;
        public Tag SelectedTag
        {
            get { return _selectedTag; }
            set
            {
                _selectedTag = value;
                FilterAssetsByTag();

            }
        }

        private List<string> _availableFormats;
        public List<string> AvailableFormats
        {
            get { return _availableFormats; }
            set { _availableFormats = value; }
        }

        private string _selectedFileToConvert;
        public string SelectedAssetToConvert
        {
            get { return _selectedFileToConvert; }
            set
            {
                _selectedFileToConvert = value;

                OnPropertyChanged(nameof(SelectedAssetToConvert));
                ExecuteSearch(SelectedAssetToConvert);

            }
        }

        private string _selectedFromFormat;
        public string SelectedFromFormat
        {
            get { return _selectedFromFormat; }
            set
            {
                _selectedFromFormat = value;
            }
        }

        private string _selectedToFormat;
        public string SelectedToFormat
        {
            get { return _selectedToFormat; }
            set
            {
                _selectedToFormat = value;
            }
        }
        public OverviewPageVM(MainPageVM mainPageVM)
        {
            MainPageVM = mainPageVM;


            Assets = new List<Asset>();
            FilteredAssets = new List<Asset>();
            AvailableFormats = new List<string>();

            SearchCommand = new RelayCommand<string>(ExecuteSearch);
            OpenHomePageCommand = new RelayCommand(OpenHomePage);
            OpenFullImageCommand = new RelayCommand(OpenFullImage);
            ClearSelectionCommand = new RelayCommand(ClearSelection);
            OpenMetadataFileCommand = new RelayCommand(OpenMetadataFile);
            RemoveAllTagsCommand = new RelayCommand(RemoveAllTags);
            AddTagCommand = new RelayCommand(AddTag);
            AddAssetTagCommand = new RelayCommand(AddAssetTag);
            RemoveTagCommand = new RelayCommand<Tag>(RemoveTag);
            RemoveAssetTagCommand = new RelayCommand<AssetTag>(RemoveAssetTag);
            ConvertCommand = new RelayCommand(ConvertFile);

            _assetRepository = new AssetRepository();

            foreach (var format in Enum.GetValues(typeof(AssetHelpers.AssetHelpers.AvailableFormats)))
            {
                AvailableFormats.Add(format.ToString());
            }
        }

        public OverviewPageVM() { }

        async void ConvertFile()
        {
            if (SelectedFromFormat == "FBX" && SelectedToFormat == "OBJ")
            {
                string objFilePath = _assetRepository.ConvertFbxToObj(SelectedAsset.FilePath);

                if (objFilePath != null)
                {
                    //TODO
                    var asset = new Asset(

                      name: Path.GetFileName(objFilePath),
                      filePath: objFilePath,
                      projectId: SelectedAsset.ProjectId,
                      fileType: AssetHelpers.AssetHelpers.AssetType.Obj,
                      relativePath: Path.GetFileName(objFilePath),
                      previewImagePath:
                      AssetHelpers.AssetHelpers.GetPlaceholderPath(SelectedToFormat));

                    asset.Metadata = new AssetMetadata
                    {
                        Name = asset.FileName,
                        FilePath = asset.FilePath,
                        FileType = asset.FileType,
                        Format = "Not defined"
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

                    if (!Assets.Any(a => a.FileName == asset.FileName && a.FilePath == asset.FilePath && a.ProjectId == asset.ProjectId))
                    {
                        _assetRepository.Assets.Add(asset);
                        Assets = _assetRepository.Assets;
                        ExecuteSearch(SearchText);
                        MainPageVM.AppDbContext.Assets.Add(asset);
                        MainPageVM.AppDbContext.SaveChanges();
                        OnPropertyChanged(nameof(Assets));
                        OnPropertyChanged(nameof(FilteredAssets));
                    }

                }


            }
            else if (SelectedFromFormat == "PNG" && SelectedToFormat == "JPG")
            {
                string jpgFilePath = _assetRepository.ConvertPngToJpg(SelectedAsset.FilePath);

                if (jpgFilePath != null)
                {
                    //TODO
                    var asset = new Asset(

                                             name: Path.GetFileName(jpgFilePath),
                                             filePath: jpgFilePath,
                                             projectId: SelectedAsset.ProjectId,
                                             fileType: AssetHelpers.AssetHelpers.AssetType.Image,
                                             relativePath: Path.GetFileName(jpgFilePath),
                                                                                                                                                      previewImagePath:
                                                                                                                                                                           AssetHelpers.AssetHelpers.GetPlaceholderPath(SelectedToFormat));

                    asset.Metadata = new AssetMetadata
                    {
                        Name = asset.FileName,
                        FilePath = asset.FilePath,
                        FileType = asset.FileType,
                        Format = "Not defined"
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

                    string extension = Path.GetExtension(Path.GetFileName(jpgFilePath)).ToLower();


                    asset.PreviewImagePath = SelectedAsset.PreviewImagePath;

                    var existingTag = MainPageVM.AppDbContext.Tags.FirstOrDefault(t => t.Name == asset.FileType.ToString());


                    if (existingTag == null)
                    {
                        existingTag = new Tag { Name = asset.FileType.ToString(), Color = AssetHelpers.AssetHelpers.GenerateRandomColorRGB().ToString() };
                        MainPageVM.AppDbContext.Tags.Add(existingTag);

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
                        MainPageVM.AppDbContext.AssetTags.Add(assetTag);

                        if (!Assets.Any(a => a.FileName == asset.FileName && a.FilePath == asset.FilePath && a.ProjectId == asset.ProjectId))
                        {
                            _assetRepository.Assets.Add(asset);
                            Assets = _assetRepository.Assets;
                            ExecuteSearch(SearchText);
                            MainPageVM.AppDbContext.Assets.Add(asset);
                            MainPageVM.AppDbContext.SaveChanges();
                            OnPropertyChanged(nameof(Assets));
                            OnPropertyChanged(nameof(FilteredAssets));
                        }

                    }
                }
            }
            else if(SelectedFromFormat == "JPG" && SelectedToFormat == "PNG")
            {
                string pngFilePath = _assetRepository.ConvertPngToJpg(SelectedAsset.FilePath);

                if (pngFilePath != null)
                {
                    //TODO
                    var asset = new Asset(

                                             name: Path.GetFileName(pngFilePath),
                                             filePath: pngFilePath,
                                             projectId: SelectedAsset.ProjectId,
                                             fileType: AssetHelpers.AssetHelpers.AssetType.Image,
                                             relativePath: Path.GetFileName(pngFilePath),
                                                                                                                                                      previewImagePath:
                                                                                                                                                                           AssetHelpers.AssetHelpers.GetPlaceholderPath(SelectedToFormat));

                    asset.Metadata = new AssetMetadata
                    {
                        Name = asset.FileName,
                        FilePath = asset.FilePath,
                        FileType = asset.FileType,
                        Format = "Not defined"
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

                    string extension = Path.GetExtension(Path.GetFileName(pngFilePath)).ToLower();


                    asset.PreviewImagePath = SelectedAsset.PreviewImagePath;

                    var existingTag = MainPageVM.AppDbContext.Tags.FirstOrDefault(t => t.Name == asset.FileType.ToString());


                    if (existingTag == null)
                    {
                        existingTag = new Tag { Name = asset.FileType.ToString(), Color = AssetHelpers.AssetHelpers.GenerateRandomColorRGB().ToString() };
                        MainPageVM.AppDbContext.Tags.Add(existingTag);

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
                        MainPageVM.AppDbContext.AssetTags.Add(assetTag);

                        if (!Assets.Any(a => a.FileName == asset.FileName && a.FilePath == asset.FilePath && a.ProjectId == asset.ProjectId))
                        {
                            _assetRepository.Assets.Add(asset);
                            Assets = _assetRepository.Assets;
                            ExecuteSearch(SearchText);
                            MainPageVM.AppDbContext.Assets.Add(asset);
                            MainPageVM.AppDbContext.SaveChanges();
                            OnPropertyChanged(nameof(Assets));
                            OnPropertyChanged(nameof(FilteredAssets));
                        }

                    }
                }
            }
            else if(SelectedFromFormat == "OBJ" && SelectedToFormat == "FBX")
            {
                string fbxFilePath = _assetRepository.ConvertObjToFbx(SelectedAsset.FilePath);

                if (fbxFilePath != null)
                {
                    //TODO
                    var asset = new Asset(

                      name: Path.GetFileName(fbxFilePath),
                      filePath: fbxFilePath,
                      projectId: SelectedAsset.ProjectId,
                      fileType: AssetHelpers.AssetHelpers.AssetType.Model,
                      relativePath: Path.GetFileName(fbxFilePath)
                     );

                    asset.PreviewImagePath = _assetRepository.GenerateThumbnail(asset, Path.GetExtension(fbxFilePath));
                    asset.Metadata = new AssetMetadata
                    {
                        Name = asset.FileName,
                        FilePath = asset.FilePath,
                        FileType = asset.FileType,
                        Format = "Not defined"
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

                    if (!Assets.Any(a => a.FileName == asset.FileName && a.FilePath == asset.FilePath && a.ProjectId == asset.ProjectId))
                    {
                        _assetRepository.Assets.Add(asset);
                        Assets = _assetRepository.Assets;
                        ExecuteSearch(SearchText);
                        MainPageVM.AppDbContext.Assets.Add(asset);
                        MainPageVM.AppDbContext.SaveChanges();
                        OnPropertyChanged(nameof(Assets));
                        OnPropertyChanged(nameof(FilteredAssets));
                    }

                }
            }
        }

        private void ExecuteSearch(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                FilteredAssets = new List<Asset>(Assets);
            }
            else
            {
                try
                {
                    var regex = new Regex("^" + Regex.Escape(searchText), RegexOptions.IgnoreCase);
                    var filtered = Assets.Where(a => regex.IsMatch(a.FileName) ||
                        a.AssetTags != null && a.AssetTags.Any(tag => regex.IsMatch(tag.Tag.Name)));

                    FilteredAssets = new List<Asset>(filtered);
                }
                catch (RegexParseException)
                {

                    FilteredAssets = new List<Asset>();
                }
            }

            OnPropertyChanged(nameof(FilteredAssets));

        }

        private void FilterAssetsByTag()
        {
            FilteredAssets = Assets
                .Where(a => a.AssetTags != null && a.AssetTags.Any(tag =>
                tag.Tag.Name.Equals(SelectedTag.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            OnPropertyChanged(nameof(FilteredAssets));

        }
        private void ClearSelection()
        {
            SelectedAsset = null;
        }
        private void OpenMetadataFile()
        {
            MainPageVM?.HandleOpenPopUpWindow(SelectedAsset);
        }

        private void OpenHomePage()
        {
            MainPageVM?.OpenHomePage();
        }

        private void OpenFullImage()
        {
            var imageViewer = new ImageViewerWindow(SelectedAsset.FilePath)
            {
                Owner = App.Current.MainWindow
            };
            imageViewer.ShowDialog();
        }

        public async Task LoadAssetsFromUnityProject(string projectPath, int currentProjectId)
        {

            Assets = await _assetRepository.LoadAssetsFromUnityProjectAsync(projectPath,
                MainPageVM.AppDbContext, currentProjectId);
            FilteredAssets = Assets;

        }

        public void LoadTags()
        {
            _tagRepository = new TagRepository(MainPageVM.AppDbContext, MainPageVM);

            LoadAllTags();
        }
        public async void LoadAllTags()
        {
            Tags = await _tagRepository.GetAllTagsAsync();
        }

        public async void RemoveAllTags()
        {
            await _tagRepository.RemoveAllTagsAsync();
            Tags = new List<Tag>();
            OnPropertyChanged(nameof(Tags));
        }
        public async Task<List<AssetTag>> LoadAssetTags(int assetId)
        {
            return await _tagRepository.GetAssetTagsAsync(assetId);
        }

        public async void AddAssetTag()
        {
            if (NewAssetTagName == null) return;

            if (!string.IsNullOrWhiteSpace(NewAssetTagName))
            {
                await _tagRepository.AddAssetTagAsync(SelectedAsset.Id, NewAssetTagName);
                NewAssetTagName = string.Empty;
                OnPropertyChanged(nameof(NewAssetTagName));
                OnPropertyChanged(nameof(SelectedAsset.AssetTags));
                LoadAllTags();
                OnPropertyChanged(nameof(Tags));
            }
        }

        public async void AddAssetTag(string assetTagName)
        {
            await _tagRepository.AddAssetTagAsync(SelectedAsset.Id, assetTagName);

            OnPropertyChanged(nameof(SelectedAsset.AssetTags));
            LoadAllTags();

        }

        public async void RemoveAssetTag(AssetTag assetTag)
        {
            await _tagRepository.RemoveTagFromAssetAsync(assetTag.AssetId, assetTag.Tag.Name);
        }
        public async void RemoveTag(Tag tag)
        {
            _tagRepository.RemoveTag(tag.Name);
            LoadAllTags();
            OnPropertyChanged(nameof(Tags));
        }
        public async void AddTag()
        {
            if (NewTagName == null) return;

            var newTag = await _tagRepository.AddTag(NewTagName);


            if (!string.IsNullOrWhiteSpace(NewTagName) && newTag != null)
            {

                Tags = Tags.Append(newTag).ToList();
                NewTagName = string.Empty;
                OnPropertyChanged(nameof(Tags));
                OnPropertyChanged(nameof(NewTagName));
            }
            else
            {
                NewTagName = string.Empty;
                OnPropertyChanged(nameof(NewTagName));

            }
        }


    }
}
